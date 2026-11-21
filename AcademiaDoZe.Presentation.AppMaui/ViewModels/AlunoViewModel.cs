
// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(AlunoId), "Id")]
public partial class AlunoViewModel : ObservableObject
{
    private readonly IAlunoService _alunoService;
    private readonly ILogradouroService _logradouroService;

    public ObservableCollection<LogradouroDto> Logradouros { get; } = new();

    private AlunoDto _aluno = new()
    {
        Nome = string.Empty,
        Cpf = string.Empty,
        Telefone = string.Empty,
        DataNascimento = DateOnly.FromDateTime(DateTime.Today),
        Numero = string.Empty
    };

    public AlunoDto Aluno
    {
        get => _aluno;
        set
        {
            if (SetProperty(ref _aluno, value))
                OnPropertyChanged(nameof(FotoSource));
        }
    }

    private string? _alunoId;

    public string? AlunoId
    {
        get => _alunoId;
        set => SetProperty(ref _alunoId, value);
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ImageSource? FotoSource
    {
        get
        {
            byte[]? conteudo = Aluno.Foto?.Conteudo;

            if (conteudo == null || conteudo.Length == 0)
                return null;

            return ImageSource.FromStream(
                () => new MemoryStream(conteudo));
        }
    }

    private LogradouroDto? _logradouroSelecionado;

    public LogradouroDto? LogradouroSelecionado
    {
        get => _logradouroSelecionado;
        set => SetProperty(ref _logradouroSelecionado, value);
    }

    private DateTime _dataNascimentoSelecionada = DateTime.Today;

    public DateTime DataNascimentoSelecionada
    {
        get => _dataNascimentoSelecionada;
        set
        {
            if (SetProperty(ref _dataNascimentoSelecionada, value))
                Aluno.DataNascimento = DateOnly.FromDateTime(value);
        }
    }

    private string _numero = string.Empty;

    public string Numero
    {
        get => _numero;
        set => SetProperty(ref _numero, value);
    }

    private string _complemento = string.Empty;

    public string Complemento
    {
        get => _complemento;
        set => SetProperty(ref _complemento, value);
    }

    private string _senha = string.Empty;

    public string Senha
    {
        get => _senha;
        set => SetProperty(ref _senha, value);
    }

    private string _confirmarSenha = string.Empty;

    public string ConfirmarSenha
    {
        get => _confirmarSenha;
        set => SetProperty(ref _confirmarSenha, value);
    }

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public AlunoViewModel(
        IAlunoService alunoService,
        ILogradouroService logradouroService)
    {
        _alunoService = alunoService;
        _logradouroService = logradouroService;
    }

    public async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        await CarregarLogradourosAsync();

        if (int.TryParse(AlunoId, out int id) && id > 0)
        {
            await CarregarAlunoAsync(id);
        }
    }

    private async Task CarregarLogradourosAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var resultado = await _logradouroService.ObterTodosAsync(
                cts.Token);

            Logradouros.Clear();

            foreach (var logradouro in resultado)
                Logradouros.Add(logradouro);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível carregar os logradouros: {ex.Message}",
                "OK");
        }
    }

    private async Task CarregarAlunoAsync(int id)
    {
        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var alunoEncontrado = await _alunoService.ObterPorIdAsync(
                id,
                cts.Token);

            if (alunoEncontrado == null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "Aluno não encontrado.",
                    "OK");

                await Shell.Current.GoToAsync("..");
                return;
            }

            Aluno = alunoEncontrado;

            Numero = Aluno.Numero ?? string.Empty;
            Complemento = Aluno.Complemento ?? string.Empty;

            DataNascimentoSelecionada =
                Aluno.DataNascimento.ToDateTime(TimeOnly.MinValue);

            // A senha não será exigida novamente na edição.
            Senha = string.Empty;
            ConfirmarSenha = string.Empty;

            IsEditMode = true;

            // Tenta recuperar o logradouro pelo endereço do DTO.
            if (Aluno.Endereco != null)
            {
                LogradouroSelecionado = Logradouros.FirstOrDefault(
                    l => l.Id == Aluno.Endereco.Id);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível carregar o aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelecionarFotoAsync()
    {
        try
        {
            var foto = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Selecione uma foto",
                    FileTypes = FilePickerFileType.Images
                });

            if (foto == null)
                return;

            await CarregarFotoAsync(foto);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível selecionar a foto: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task TirarFotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Câmera indisponível",
                    "Este dispositivo não oferece suporte à captura de fotos.",
                    "OK");

                return;
            }

            var foto = await MediaPicker.Default.CapturePhotoAsync();

            if (foto == null)
                return;

            await CarregarFotoAsync(foto);
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Câmera indisponível",
                "A câmera não é compatível com este dispositivo.",
                "OK");
        }
        catch (PermissionException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Permissão necessária",
                "Permita o acesso à câmera para tirar uma foto.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível tirar a foto: {ex.Message}",
                "OK");
        }
    }

    private async Task CarregarFotoAsync(FileResult foto)
    {
        await using var stream = await foto.OpenReadAsync();

        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        byte[] conteudo = memoryStream.ToArray();

        if (conteudo.Length > 5 * 1024 * 1024)
        {
            await Shell.Current.DisplayAlertAsync(
                "Foto muito grande",
                "A foto não pode ultrapassar 5 MB.",
                "OK");

            return;
        }

        Aluno.Foto = new ArquivoDto
        {
            Conteudo = conteudo
        };

        OnPropertyChanged(nameof(FotoSource));
    }

    [RelayCommand]
    private async Task RemoverFotoAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Remover foto",
            "Deseja remover a foto do aluno?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        Aluno.Foto = null;

        OnPropertyChanged(nameof(FotoSource));
    }

    [RelayCommand]
    private async Task SaveAlunoAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Aluno.Nome) ||
            string.IsNullOrWhiteSpace(Aluno.Cpf) ||
            string.IsNullOrWhiteSpace(Aluno.Telefone) ||
            string.IsNullOrWhiteSpace(Aluno.Email))
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "Preencha nome, CPF, telefone e e-mail.",
                "OK");

            return;
        }

        if (LogradouroSelecionado == null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "Selecione um logradouro cadastrado.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(Numero))
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "Informe o número da residência.",
                "OK");

            return;
        }

        if (!IsEditMode && string.IsNullOrWhiteSpace(Senha))
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "Informe uma senha.",
                "OK");

            return;
        }

        if (!string.IsNullOrWhiteSpace(Senha) &&
            Senha != ConfirmarSenha)
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "As senhas não coincidem.",
                "OK");

            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(15));

            Aluno.Endereco = LogradouroSelecionado;
            Aluno.Numero = Numero.Trim();
            Aluno.Complemento = Complemento?.Trim();

            Aluno.DataNascimento =
                DateOnly.FromDateTime(DataNascimentoSelecionada);

            // Só altera a senha se o usuário informar uma nova.
            if (!string.IsNullOrWhiteSpace(Senha))
                Aluno.Senha = Senha;

            if (IsEditMode)
            {
                await _alunoService.AtualizarAsync(
                    Aluno,
                    cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Aluno atualizado com sucesso!",
                    "OK");
            }
            else
            {
                await _alunoService.AdicionarAsync(
                    Aluno,
                    cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Aluno cadastrado com sucesso!",
                    "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "A operação demorou demais. Tente novamente.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                IsEditMode ? "Erro ao atualizar" : "Erro ao cadastrar",
                ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
