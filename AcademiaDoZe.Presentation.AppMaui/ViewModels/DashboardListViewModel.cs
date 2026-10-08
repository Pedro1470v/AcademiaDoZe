// Pedro Vinicius

using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class DashboardListViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;

    private int _totalLogradouros;

    public int TotalLogradouros
    {
        get => _totalLogradouros;
        set => SetProperty(ref _totalLogradouros, value);
    }

    public DashboardListViewModel(
        ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;

        Title = "Dashboard";
    }

    [RelayCommand]
    private async Task LoadDashboardDataAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            var logradouros =
                await _logradouroService.ObterTodosAsync(cts.Token);

            TotalLogradouros = logradouros.Count();
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "Não foi possível atualizar a quantidade de logradouros.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao atualizar o Dashboard: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToLogradourosAsync()
    {
        await Shell.Current.GoToAsync("logradouros");
    }
}