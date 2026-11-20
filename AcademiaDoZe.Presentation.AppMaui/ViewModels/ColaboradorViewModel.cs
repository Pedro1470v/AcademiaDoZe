
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ILogradouroService _logradouroService;

    public IEnumerable<EColaboradorTipo> ColaboradorTipos { get; } =
        Enum.GetValues<EColaboradorTipo>();

    public IEnumerable<EColaboradorVinculo> ColaboradorVinculos { get; } =
        Enum.GetValues<EColaboradorVinculo>();

    private ColaboradorDto _colaborador = CriarColaboradorPadrao();

    public ColaboradorDto Colaborador
    {
        get => _colaborador;
        set
        {
            if (SetProperty(ref _colaborador, value))
            {
                OnPropertyChanged(nameof(HasEnderecoVinculado));
                OnPropertyChanged(nameof(HasNoEnderecoVinculado));
            }
        }
    }

    private int _colaboradorId;

    public int ColaboradorId
    {
        get => _colaboradorId;
        set => SetProperty(ref _colaboradorId, value);
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    private string _confirmarSenha = string.Empty;

    public string ConfirmarSenha
    {
        get => _confirmarSenha;
        set => SetProperty(ref _confirmarSenha, value);
    }

    public bool HasEnderecoVinculado =>
        Colaborador?.Endereco != null && Colaborador.Endereco.Id > 0;

    public bool HasNoEnderecoVinculado => !HasEnderecoVinculado;

    public ColaboradorViewModel(
        IColaboradorService colaboradorService,
        ILogradouroService logradouroService)
    {
        _colaboradorService = colaboradorService
            ?? throw new ArgumentNullException(nameof(colaboradorService));

        _logradouroService = logradouroService
            ?? throw new ArgumentNullException(nameof(logradouroService));

        Title = "Detalhes do Colaborador";
    }

    public async Task InitializeAsync()
    {
        ConfirmarSenha = string.Empty;

        if (ColaboradorId > 0)
        {
            IsEditMode = true;
            Title = "Editar Colaborador";
            await LoadColaboradorAsync();
        }
        else
        {
            IsEditMode = false;
            Title = "Novo Colaborador";
            Colaborador = CriarColaboradorPadrao();
            NotificarStatusEndereco();
        }
    }

    private void NotificarStatusEndereco()
    {
        OnPropertyChanged(nameof(HasEnderecoVinculado));
        OnPropertyChanged(nameof(HasNoEnderecoVinculado));
    }

    private static ColaboradorDto CriarColaboradorPadrao() => new()
    {
        Nome = string.Empty,
        Cpf = string.Empty,
        DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
        Telefone = string.Empty,
        Email = string.Empty,
        Senha = string.Empty,
        Endereco = new LogradouroDto
        {
            Cep = string.Empty,
            Nome = string.Empty,
            Bairro = string.Empty,
            Cidade = string.Empty,
            Estado = string.Empty
        },
        Numero = string.Empty,
        Complemento = string.Empty,
        DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
        Tipo = EColaboradorTipo.Atendente,
        Vinculo = EColaboradorVinculo.CLT
    };

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task LoadColaboradorAsync()
    {
        if (ColaboradorId <= 0)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var colaboradorData =
                await _colaboradorService.ObterPorIdAsync(
                    ColaboradorId, cts.Token);

            if (colaboradorData is not null)
            {
                colaboradorData.Endereco ??= new LogradouroDto
                {
                    Cep = string.Empty,
                    Nome = string.Empty,
                    Bairro = string.Empty,
                    Cidade = string.Empty,
                    Estado = string.Empty
                };

                colaboradorData.Senha = string.Empty;
                ConfirmarSenha = string.Empty;
                Colaborador = colaboradorData;

                NotificarStatusEndereco();
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "O carregamento do colaborador expirou. Verifique a conexão.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao carregar colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCpfAsync()
    {
        if (string.IsNullOrWhiteSpace(Colaborador.Cpf))
        {
            await Shell.Current.DisplayAlert(
                "Aviso",
                "Informe o CPF para realizar a busca.",
                "OK");
            return;
        }

        string apenasDigitos =
            new(Colaborador.Cpf.Where(char.IsDigit).ToArray());

        if (apenasDigitos.Length != 11)
        {
            await Shell.Current.DisplayAlert(
                "Validação",
                "O CPF deve conter exatamente 11 dígitos numéricos.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var colaboradorData =
                await _colaboradorService.ObterPorCpfAsync(
                    apenasDigitos, cts.Token);

            if (colaboradorData is not null)
            {
                colaboradorData.Endereco ??= new LogradouroDto
                {
                    Cep = string.Empty,
                    Nome = string.Empty,
                    Bairro = string.Empty,
                    Cidade = string.Empty,
                    Estado = string.Empty
                };

                colaboradorData.Senha = string.Empty;
                ConfirmarSenha = string.Empty;
                Colaborador = colaboradorData;
                ColaboradorId = colaboradorData.Id;
                IsEditMode = true;
                Title = "Editar Colaborador";

                NotificarStatusEndereco();

                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Colaborador já cadastrado! Dados carregados para edição.",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "CPF não encontrado.",
                    "OK");
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "A busca por CPF expirou. Verifique a conexão.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao buscar CPF: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCepAsync()
    {
        if (Colaborador.Endereco is null ||
            string.IsNullOrWhiteSpace(Colaborador.Endereco.Cep))
        {
            await Shell.Current.DisplayAlert(
                "Aviso",
                "Informe o CEP para realizar a busca.",
                "OK");
            return;
        }

        string apenasDigitos =
            new(Colaborador.Endereco.Cep.Where(char.IsDigit).ToArray());

        if (apenasDigitos.Length != 8)
        {
            await Shell.Current.DisplayAlert(
                "Validação",
                "O CEP deve conter exatamente 8 dígitos numéricos.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var logradouros =
                await _logradouroService.ObterPorCepAsync(
                    apenasDigitos, cts.Token);

            var logradouroData = logradouros?
                .FirstOrDefault(l =>
                    new string((l.Cep ?? string.Empty)
                        .Where(char.IsDigit)
                        .ToArray()) == apenasDigitos);

            if (logradouroData is not null)
            {
                Colaborador.Endereco = logradouroData;
                OnPropertyChanged(nameof(Colaborador));

                NotificarStatusEndereco();

                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "CEP encontrado! Endereço preenchido automaticamente.",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "CEP não encontrado no cadastro de logradouros. Cadastre o logradouro primeiro.",
                    "OK");
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "A busca de CEP expirou. Verifique a conexão com o banco.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao buscar CEP: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveColaboradorAsync()
    {
        if (IsBusy)
            return;

        if (!await ValidateColaboradorAsync(
                Colaborador, IsEditMode, ConfirmarSenha))
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (Colaborador.Endereco is not null &&
                Colaborador.Endereco.Id <= 0 &&
                !string.IsNullOrWhiteSpace(Colaborador.Endereco.Cep))
            {
                string cepLimpo =
                    new(Colaborador.Endereco.Cep
                        .Where(char.IsDigit).ToArray());

                if (cepLimpo.Length != 8)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe um CEP válido com 8 dígitos para o endereço.",
                        "OK");
                    return;
                }

                var logradouros =
                    await _logradouroService.ObterPorCepAsync(
                        cepLimpo, cts.Token);

                var logradouroData = logradouros?
                    .FirstOrDefault(l =>
                        new string((l.Cep ?? string.Empty)
                            .Where(char.IsDigit)
                            .ToArray()) == cepLimpo);

                if (logradouroData is null)
                {
                    await Shell.Current.DisplayAlert(
                        "Endereço Obrigatório",
                        "O CEP informado não está cadastrado como Logradouro. Cadastre-o previamente.",
                        "OK");
                    return;
                }

                Colaborador.Endereco = logradouroData;
            }

            Colaborador.Nome = Colaborador.Nome.Trim();

            Colaborador.Cpf =
                new(Colaborador.Cpf.Where(char.IsDigit).ToArray());

            Colaborador.Telefone =
                new(Colaborador.Telefone.Where(char.IsDigit).ToArray());

            Colaborador.Numero = Colaborador.Numero.Trim();

            Colaborador.Complemento =
                string.IsNullOrWhiteSpace(Colaborador.Complemento)
                    ? null
                    : Colaborador.Complemento.Trim();

            Colaborador.Email =
                string.IsNullOrWhiteSpace(Colaborador.Email)
                    ? null
                    : Colaborador.Email.Trim();

            Colaborador.Senha =
                string.IsNullOrWhiteSpace(Colaborador.Senha)
                    ? null
                    : Colaborador.Senha.Trim();

            if (IsEditMode)
            {
                await _colaboradorService.AtualizarAsync(
                    Colaborador, cts.Token);

                await Shell.Current.DisplayAlert(
                    "Sucesso",
                    "Colaborador atualizado com sucesso!",
                    "OK");
            }
            else
            {
                await _colaboradorService.AdicionarAsync(
                    Colaborador, cts.Token);

                await Shell.Current.DisplayAlert(
                    "Sucesso",
                    "Colaborador criado com sucesso!",
                    "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "A gravação do colaborador expirou. Verifique a conexão.",
                "OK");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert(
                "Regra de Negócio", ex.Message, "OK");
        }
        catch (ArgumentException ex)
        {
            await Shell.Current.DisplayAlert(
                "Validação", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao salvar colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelecionarFotoAsync()
    {
        try
        {
            string escolha =
                await Shell.Current.DisplayActionSheet(
                    "Origem da Imagem",
                    "Cancelar",
                    "Remover Foto",
                    "Galeria",
                    "Câmera");

            if (escolha == "Remover Foto")
            {
                Colaborador.Foto = null;
                OnPropertyChanged(nameof(Colaborador));
                return;
            }

            FileResult? result = null;

            if (escolha == "Galeria")
            {
                result = await FilePicker.Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Selecione uma imagem",
                        FileTypes = FilePickerFileType.Images
                    });
            }
            else if (escolha == "Câmera")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    result =
                        await MediaPicker.Default.CapturePhotoAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Erro",
                        "Captura de foto não suportada neste dispositivo.",
                        "OK");
                    return;
                }
            }

            if (result is not null)
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();

                await stream.CopyToAsync(ms);

                Colaborador.Foto = new ArquivoDto
                {
                    Conteudo = ms.ToArray()
                };

                OnPropertyChanged(nameof(Colaborador));
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao selecionar imagem: {ex.Message}",
                "OK");
        }
    }

    private static async Task<bool> ValidateColaboradorAsync(
        ColaboradorDto colaborador,
        bool isEditMode,
        string confirmarSenha)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(colaborador.Nome))
            errors.Add("• Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(colaborador.Cpf))
        {
            errors.Add("• CPF é obrigatório.");
        }
        else
        {
            string cpf =
                new(colaborador.Cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                errors.Add("• CPF deve conter exatamente 11 dígitos numéricos.");
        }

        if (colaborador.DataNascimento == default)
        {
            errors.Add("• Data de nascimento é obrigatória.");
        }
        else
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);

            if (colaborador.DataNascimento > hoje)
                errors.Add("• Data de nascimento não pode ser futura.");
            else if (colaborador.DataNascimento >
                     DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
                errors.Add("• O colaborador deve ter no mínimo 12 anos de idade.");
        }

        if (string.IsNullOrWhiteSpace(colaborador.Telefone))
        {
            errors.Add("• Telefone é obrigatório.");
        }
        else
        {
            string telefone =
                new(colaborador.Telefone.Where(char.IsDigit).ToArray());

            if (telefone.Length != 11)
                errors.Add("• Telefone deve conter 11 dígitos numéricos com DDD.");
        }

        if (!string.IsNullOrWhiteSpace(colaborador.Email) &&
            !ValidarFormatoEmail(colaborador.Email.Trim()))
        {
            errors.Add("• Formato de e-mail inválido.");
        }

        if (!isEditMode)
        {
            if (string.IsNullOrWhiteSpace(colaborador.Senha))
            {
                errors.Add("• Senha é obrigatória no cadastro.");
            }
            else
            {
                string senha = colaborador.Senha.Trim();

                if (senha.Length < 6 || !senha.Any(char.IsUpper))
                    errors.Add("• A senha deve ter no mínimo 6 caracteres e pelo menos uma letra maiúscula.");

                if (colaborador.Senha != confirmarSenha)
                    errors.Add("• A confirmação de senha não confere com a senha informada.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(colaborador.Senha))
        {
            string senha = colaborador.Senha.Trim();

            if (senha.Length < 6 || !senha.Any(char.IsUpper))
                errors.Add("• A nova senha deve ter no mínimo 6 caracteres e pelo menos uma letra maiúscula.");

            if (colaborador.Senha != confirmarSenha)
                errors.Add("• A confirmação de senha não confere com a nova senha informada.");
        }

        if (colaborador.Endereco is null ||
            string.IsNullOrWhiteSpace(colaborador.Endereco.Cep))
        {
            errors.Add("• CEP do endereço é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(colaborador.Numero))
            errors.Add("• Número do endereço é obrigatório.");

        if (colaborador.DataAdmissao == default)
        {
            errors.Add("• Data de admissão é obrigatória.");
        }
        else if (colaborador.DataAdmissao >
                 DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("• Data de admissão não pode ser maior que a data atual.");
        }

        if (colaborador.Tipo == EColaboradorTipo.Administrador &&
            colaborador.Vinculo != EColaboradorVinculo.CLT)
        {
            errors.Add("• Colaborador com tipo Administrador deve ter vínculo CLT.");
        }

        if (errors.Count > 0)
        {
            string mensagem =
                "Por favor, corrija os seguintes campos:\n\n" +
                string.Join("\n", errors);

            await Shell.Current.DisplayAlert(
                "Erros de Validação", mensagem, "OK");

            return false;
        }

        return true;
    }

    private static bool ValidarFormatoEmail(string email)
    {
        string[] partes = email.Split('@');

        if (partes.Length != 2)
            return false;

        if (string.IsNullOrWhiteSpace(partes[0]))
            return false;

        string dominio = partes[1];

        if (string.IsNullOrWhiteSpace(dominio) ||
            dominio.StartsWith('.') ||
            dominio.EndsWith('.'))
            return false;

        string[] labels = dominio.Split('.');

        if (labels.Length < 2)
            return false;

        return !labels.Any(string.IsNullOrWhiteSpace);
    }
}
