
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

    private ColaboradorDto _colaborador = CriarColaboradorPadrao();
    private int _colaboradorId;
    private bool _isEditMode;
    private string _confirmarSenha = string.Empty;

    public IEnumerable<EColaboradorTipo> ColaboradorTipos { get; } =
        Enum.GetValues<EColaboradorTipo>();

    public IEnumerable<EColaboradorVinculo> ColaboradorVinculos { get; } =
        Enum.GetValues<EColaboradorVinculo>();

    public ColaboradorDto Colaborador
    {
        get => _colaborador;
        set
        {
            if (SetProperty(ref _colaborador, value))
                NotificarStatusEndereco();
        }
    }

    public int ColaboradorId
    {
        get => _colaboradorId;
        set => SetProperty(ref _colaboradorId, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

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

        Title = "Novo Colaborador";
    }

    private static ColaboradorDto CriarColaboradorPadrao()
    {
        return new ColaboradorDto
        {
            Nome = string.Empty,
            Cpf = string.Empty,
            DataNascimento =
                DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
            Telefone = string.Empty,
            Email = string.Empty,
            Senha = string.Empty,
            Endereco = new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil"
            },
            Numero = string.Empty,
            Complemento = string.Empty,
            DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
            Tipo = EColaboradorTipo.Atendente,
            Vinculo = EColaboradorVinculo.CLT
        };
    }

    private void NotificarStatusEndereco()
    {
        OnPropertyChanged(nameof(HasEnderecoVinculado));
        OnPropertyChanged(nameof(HasNoEnderecoVinculado));
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
        }

        NotificarStatusEndereco();
    }

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

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var dados = await _colaboradorService.ObterPorIdAsync(
                ColaboradorId, cts.Token);

            if (dados == null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Colaborador não encontrado.",
                    "OK");
                return;
            }

            dados.Endereco ??= new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil"
            };

            // Não exibe a senha armazenada ao editar.
            dados.Senha = string.Empty;

            Colaborador = dados;
            ConfirmarSenha = string.Empty;
            IsEditMode = true;
            Title = "Editar Colaborador";
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "O carregamento do colaborador demorou demais.",
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

        string cpf = SomenteDigitos(Colaborador.Cpf);

        if (cpf.Length != 11)
        {
            await Shell.Current.DisplayAlert(
                "Validação",
                "O CPF deve conter 11 dígitos numéricos.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var dados = await _colaboradorService.ObterPorCpfAsync(
                cpf, cts.Token);

            if (dados == null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Nenhum colaborador foi encontrado com esse CPF.",
                    "OK");
                return;
            }

            dados.Endereco ??= new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil"
            };

            dados.Senha = string.Empty;

            Colaborador = dados;
            ColaboradorId = dados.Id;
            ConfirmarSenha = string.Empty;
            IsEditMode = true;
            Title = "Editar Colaborador";

            NotificarStatusEndereco();

            await Shell.Current.DisplayAlert(
                "Colaborador encontrado",
                "Os dados foram carregados para edição.",
                "OK");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "A busca por CPF demorou demais.",
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
        if (Colaborador.Endereco == null ||
            string.IsNullOrWhiteSpace(Colaborador.Endereco.Cep))
        {
            await Shell.Current.DisplayAlert(
                "Aviso",
                "Informe o CEP para realizar a busca.",
                "OK");
            return;
        }

        string cep = SomenteDigitos(Colaborador.Endereco.Cep);

        if (cep.Length != 8)
        {
            await Shell.Current.DisplayAlert(
                "Validação",
                "O CEP deve conter 8 dígitos numéricos.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            IEnumerable<LogradouroDto>? logradouros =
                await _logradouroService.ObterPorCepAsync(
                    cep, cts.Token);

            var logradouro = logradouros?
                .FirstOrDefault(x => x.Id > 0);

            if (logradouro == null)
            {
                await Shell.Current.DisplayAlert(
                    "Endereço não encontrado",
                    "Não foi encontrado um logradouro cadastrado para esse CEP. Cadastre o logradouro antes de continuar.",
                    "OK");
                return;
            }

            // Mantém o número e o complemento digitados pelo usuário.
            Colaborador.Endereco = logradouro;
            OnPropertyChanged(nameof(Colaborador));
            NotificarStatusEndereco();

            await Shell.Current.DisplayAlert(
                "Endereço encontrado",
                "O logradouro foi vinculado ao colaborador.",
                "OK");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "A busca por CEP demorou demais.",
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
        {
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            // Confere o CEP e recupera um logradouro realmente cadastrado.
            if (Colaborador.Endereco == null)
            {
                await Shell.Current.DisplayAlert(
                    "Endereço obrigatório",
                    "Informe um CEP e clique em Buscar CEP.",
                    "OK");
                return;
            }

            string cep = SomenteDigitos(Colaborador.Endereco.Cep);

            if (cep.Length != 8)
            {
                await Shell.Current.DisplayAlert(
                    "CEP inválido",
                    "Informe um CEP válido e clique em Buscar CEP.",
                    "OK");
                return;
            }

            IEnumerable<LogradouroDto>? logradouros =
                await _logradouroService.ObterPorCepAsync(
                    cep, cts.Token);

            var logradouro = logradouros?
                .FirstOrDefault(x => x.Id > 0);

            if (logradouro == null)
            {
                await Shell.Current.DisplayAlert(
                    "Endereço não encontrado",
                    "Esse CEP não corresponde a um logradouro cadastrado. Cadastre-o primeiro.",
                    "OK");
                return;
            }

            // A entidade precisa receber um endereço com ID válido.
            Colaborador.Endereco = logradouro;

            Colaborador.Nome = Colaborador.Nome.Trim();
            Colaborador.Cpf = SomenteDigitos(Colaborador.Cpf);
            Colaborador.Telefone = SomenteDigitos(Colaborador.Telefone);
            Colaborador.Endereco.Cep = cep;
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
                    "Colaborador cadastrado com sucesso!",
                    "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "A gravação demorou demais. Tente novamente.",
                "OK");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert(
                "Regra de negócio",
                ex.Message,
                "OK");
        }
        catch (ArgumentException ex)
        {
            await Shell.Current.DisplayAlert(
                "Validação",
                ex.Message,
                "OK");
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
            string escolha = await Shell.Current.DisplayActionSheet(
                "Origem da imagem",
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

            FileResult? resultado = null;

            if (escolha == "Galeria")
            {
                resultado = await FilePicker.Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Selecione uma imagem",
                        FileTypes = FilePickerFileType.Images
                    });
            }
            else if (escolha == "Câmera")
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Shell.Current.DisplayAlert(
                        "Aviso",
                        "A captura de fotos não é suportada neste dispositivo.",
                        "OK");
                    return;
                }

                resultado = await MediaPicker.Default.CapturePhotoAsync();
            }

            if (resultado == null)
                return;

            using var stream = await resultado.OpenReadAsync();
            using var memoria = new MemoryStream();

            await stream.CopyToAsync(memoria);

            Colaborador.Foto = new ArquivoDto
            {
                Conteudo = memoria.ToArray()
            };

            OnPropertyChanged(nameof(Colaborador));
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
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(colaborador.Nome))
            erros.Add("• Nome é obrigatório.");

        string cpf = SomenteDigitos(colaborador.Cpf);

        if (cpf.Length != 11)
            erros.Add("• CPF deve conter 11 dígitos numéricos.");

        if (colaborador.DataNascimento == default)
        {
            erros.Add("• Data de nascimento é obrigatória.");
        }
        else
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);

            if (colaborador.DataNascimento > hoje)
                erros.Add("• Data de nascimento não pode ser futura.");
            else if (colaborador.DataNascimento >
                     DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
                erros.Add("• O colaborador deve ter no mínimo 12 anos.");
        }

        string telefone = SomenteDigitos(colaborador.Telefone);

        if (telefone.Length != 11)
            erros.Add("• Telefone deve conter 11 dígitos numéricos com DDD.");

        if (!string.IsNullOrWhiteSpace(colaborador.Email) &&
            !ValidarFormatoEmail(colaborador.Email.Trim()))
        {
            erros.Add("• Formato de e-mail inválido.");
        }

        if (!isEditMode)
        {
            if (string.IsNullOrWhiteSpace(colaborador.Senha))
            {
                erros.Add("• Senha é obrigatória no cadastro.");
            }
            else
            {
                ValidarSenha(
                    colaborador.Senha,
                    confirmarSenha,
                    "Senha",
                    erros);
            }
        }
        else if (!string.IsNullOrWhiteSpace(colaborador.Senha))
        {
            ValidarSenha(
                colaborador.Senha,
                confirmarSenha,
                "Nova senha",
                erros);
        }

        if (colaborador.Endereco == null ||
            string.IsNullOrWhiteSpace(colaborador.Endereco.Cep))
        {
            erros.Add("• CEP do endereço é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(colaborador.Numero))
            erros.Add("• Número do endereço é obrigatório.");

        if (colaborador.DataAdmissao == default)
        {
            erros.Add("• Data de admissão é obrigatória.");
        }
        else if (colaborador.DataAdmissao >
                 DateOnly.FromDateTime(DateTime.Today))
        {
            erros.Add("• Data de admissão não pode ser futura.");
        }

        if (colaborador.Tipo == EColaboradorTipo.Administrador &&
            colaborador.Vinculo != EColaboradorVinculo.CLT)
        {
            erros.Add("• Administrador deve ter vínculo CLT.");
        }

        if (erros.Count > 0)
        {
            await Shell.Current.DisplayAlert(
                "Erros de validação",
                "Corrija os seguintes campos:\n\n" +
                string.Join("\n", erros),
                "OK");

            return false;
        }

        return true;
    }

    private static void ValidarSenha(
        string senha,
        string confirmarSenha,
        string descricao,
        List<string> erros)
    {
        string senhaLimpa = senha.Trim();

        if (senhaLimpa.Length < 6 ||
            !senhaLimpa.Any(char.IsUpper))
        {
            erros.Add(
                $"• {descricao} deve ter no mínimo 6 caracteres e pelo menos uma letra maiúscula.");
        }

        if (senha != confirmarSenha)
            erros.Add($"• A confirmação da {descricao.ToLowerInvariant()} não confere.");
    }

    private static string SomenteDigitos(string? valor)
    {
        return new string(
            (valor ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
    }

    private static bool ValidarFormatoEmail(string email)
    {
        var partes = email.Split('@');

        if (partes.Length != 2 ||
            string.IsNullOrWhiteSpace(partes[0]))
        {
            return false;
        }

        string dominio = partes[1];

        if (string.IsNullOrWhiteSpace(dominio) ||
            dominio.StartsWith('.') ||
            dominio.EndsWith('.'))
        {
            return false;
        }

        var partesDominio = dominio.Split('.');

        return partesDominio.Length >= 2 &&
               !partesDominio.Any(string.IsNullOrWhiteSpace);
    }
}
