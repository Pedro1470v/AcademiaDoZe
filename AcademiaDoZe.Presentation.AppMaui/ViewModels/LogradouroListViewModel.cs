using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class LogradouroListViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;

    public ObservableCollection<LogradouroDto> Logradouros { get; } = [];

    public ObservableCollection<string> FilterTypes { get; } =
        ["Cidade", "Id", "Cep"];

    private LogradouroDto? _selectedLogradouro;

    public LogradouroDto? SelectedLogradouro
    {
        get => _selectedLogradouro;
        set => SetProperty(ref _selectedLogradouro, value);
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Cidade";

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    public IAsyncRelayCommand LoadLogradourosCommand { get; }

    public LogradouroListViewModel(
        ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;
        Title = "Logradouros";

        LoadLogradourosCommand =
            new AsyncRelayCommand(LoadLogradourosAsync);
    }

    private async Task LoadLogradourosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(5));

            var logradouros =
                await _logradouroService.ObterTodosAsync(
                    cts.Token);

            Logradouros.Clear();

            foreach (var logradouro in logradouros)
            {
                Logradouros.Add(logradouro);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao carregar logradouros: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadLogradourosAsync();
    }

    [RelayCommand]
    private async Task AddLogradouroAsync()
    {
        await Shell.Current.GoToAsync("logradouro");
    }

    [RelayCommand]
    private async Task EditLogradouroAsync(
        LogradouroDto logradouro)
    {
        if (logradouro == null)
            return;

        await Shell.Current.GoToAsync(
            $"logradouro?Id={logradouro.Id}");
    }

    [RelayCommand]
    private async Task DeleteLogradouroAsync(
        LogradouroDto logradouro)
    {
        if (logradouro == null)
            return;

        bool confirmar =
            await Shell.Current.DisplayAlertAsync(
                "Confirmar Exclusão",
                $"Deseja realmente excluir o logradouro {logradouro.Nome}?",
                "Sim",
                "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(5));

            bool removido =
                await _logradouroService.RemoverAsync(
                    logradouro.Id,
                    cts.Token);

            if (removido)
            {
                Logradouros.Remove(logradouro);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Logradouro excluído com sucesso!",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao excluir logradouro: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchLogradourosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(5));

            IEnumerable<LogradouroDto> resultados = [];

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                resultados =
                    await _logradouroService.ObterTodosAsync(
                        cts.Token);
            }
            else if (SelectedFilterType == "Cidade")
            {
                resultados =
                    await _logradouroService.ObterPorCidadeAsync(
                        SearchText.Trim(),
                        cts.Token);
            }
            else if (SelectedFilterType == "Id")
            {
                if (int.TryParse(
                    SearchText.Trim(),
                    out int id) && id > 0)
                {
                    var logradouro =
                        await _logradouroService.ObterPorIdAsync(
                            id,
                            cts.Token);

                    if (logradouro != null)
                    {
                        resultados = [logradouro];
                    }
                }
            }
            else if (SelectedFilterType == "Cep")
            {
                var cep =
                    new string(
                        [.. SearchText.Where(char.IsDigit)]);

                if (cep.Length == 8)
                {
                    resultados =
                        await _logradouroService.ObterPorCepAsync(
                            cep,
                            cts.Token);
                }
            }

            Logradouros.Clear();

            foreach (var logradouro in resultados)
            {
                Logradouros.Add(logradouro);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao buscar logradouros: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}