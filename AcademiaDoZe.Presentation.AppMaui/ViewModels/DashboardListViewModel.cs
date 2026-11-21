
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        _logradouroService = logradouroService
            ?? throw new ArgumentNullException(nameof(logradouroService));

        Title = "Dashboard";
    }

    [RelayCommand]
    public async Task LoadDashboardDataAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var logradouros = await _logradouroService
                .ObterTodosAsync(cancellationTokenSource.Token);

            TotalLogradouros = logradouros?.Count() ?? 0;
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "Não foi possível carregar os logradouros no tempo esperado.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro ao atualizar o Dashboard",
                ex.Message,
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

    [RelayCommand]
    private async Task NavigateToAlunosAsync()
    {
        await Shell.Current.GoToAsync("alunos");
    }

    [RelayCommand]
    private async Task NavigateToMatriculasAsync()
    {
        await Shell.Current.GoToAsync("matriculas");
    }

    [RelayCommand]
    private async Task NavigateToColaboradoresAsync()
    {
        await Shell.Current.GoToAsync("colaboradores");
    }
}
