// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(LogradouroId), "Id")]
public partial class LogradouroViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;

    private LogradouroDto _logradouro = new()
    {
        Cep = string.Empty,
        Nome = string.Empty,
        Bairro = string.Empty,
        Cidade = string.Empty,
        Estado = string.Empty
    };

    public LogradouroDto Logradouro
    {
        get => _logradouro;
        set => SetProperty(ref _logradouro, value);
    }

    private int _logradouroId;

    public int LogradouroId
    {
        get => _logradouroId;
        set
        {
            if (SetProperty(ref _logradouroId, value) && value > 0)
                IsEditMode = true;
        }
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public LogradouroViewModel(ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;
        Title = "Detalhes do Logradouro";
    }

    public async Task InitializeAsync()
    {
        if (LogradouroId > 0)
        {
            IsEditMode = true;
            Title = "Editar Logradouro";
            await LoadLogradouroAsync();
        }
        else
        {
            IsEditMode = false;
            Title = "Novo Logradouro";

            Logradouro = new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty
            };
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task LoadLogradouroAsync()
    {
        if (LogradouroId <= 0)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            var existente = await _logradouroService.ObterPorIdAsync(
                LogradouroId,
                cts.Token);

            if (existente == null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "Logradouro não encontrado.",
                    "OK");
                return;
            }

            Logradouro = existente;
            LogradouroId = existente.Id;
            IsEditMode = true;
            Title = "Editar Logradouro";
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "O carregamento do logradouro expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao carregar logradouro: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchByCepAsync()
    {
        if (string.IsNullOrWhiteSpace(Logradouro.Cep))
        {
            await Shell.Current.DisplayAlertAsync(
                "Aviso",
                "Informe o CEP para realizar a busca.",
                "OK");
            return;
        }

        var cep = new string(
            Logradouro.Cep.Where(char.IsDigit).ToArray());

        if (cep.Length != 8)
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "O CEP deve conter exatamente 8 dígitos.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            var resultados = await _logradouroService.ObterPorCepAsync(
                cep,
                cts.Token);

            var existente = resultados.FirstOrDefault();

            if (existente != null)
            {
                Logradouro = existente;
                LogradouroId = existente.Id;
                IsEditMode = true;
                Title = "Editar Logradouro";

                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "CEP já cadastrado. Dados carregados para edição.",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "CEP não encontrado. Você pode cadastrar um novo logradouro.",
                    "OK");
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "A busca pelo CEP expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
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
    private async Task SaveLogradouroAsync()
    {
        if (string.IsNullOrWhiteSpace(Logradouro.Cep) ||
            string.IsNullOrWhiteSpace(Logradouro.Nome) ||
            string.IsNullOrWhiteSpace(Logradouro.Bairro) ||
            string.IsNullOrWhiteSpace(Logradouro.Cidade) ||
            string.IsNullOrWhiteSpace(Logradouro.Estado))
        {
            await Shell.Current.DisplayAlertAsync(
                "Validação",
                "Preencha todos os campos obrigatórios.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            if (IsEditMode)
            {
                if (LogradouroId <= 0)
                {
                    throw new InvalidOperationException(
                        "O ID do logradouro para edição é inválido.");
                }

                Logradouro.Id = LogradouroId;

                await _logradouroService.AtualizarAsync(
                    Logradouro,
                    cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Logradouro atualizado com sucesso!",
                    "OK");
            }
            else
            {
                await _logradouroService.AdicionarAsync(
                    Logradouro,
                    cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Logradouro cadastrado com sucesso!",
                    "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "A operação expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao salvar logradouro: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}