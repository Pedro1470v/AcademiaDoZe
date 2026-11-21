
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorListViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;

    public ObservableCollection<string> FilterTypes { get; } =
        ["Nome", "CPF", "Id", "Email", "Tipo", "Vínculo"];

    private ObservableCollection<ColaboradorDto> _colaboradores = [];

    public ObservableCollection<ColaboradorDto> Colaboradores
    {
        get => _colaboradores;
        set => SetProperty(ref _colaboradores, value);
    }

    private ColaboradorDto? _selectedColaborador;

    public ColaboradorDto? SelectedColaborador
    {
        get => _selectedColaborador;
        set => SetProperty(ref _selectedColaborador, value);
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Nome";

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    public ColaboradorListViewModel(
        IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService
            ?? throw new ArgumentNullException(nameof(colaboradorService));

        Title = "Colaboradores";
    }

    [RelayCommand]
    private async Task AddColaboradorAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("colaborador");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao navegar para tela de cadastro: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task EditColaboradorAsync(ColaboradorDto colaborador)
    {
        if (colaborador == null)
            return;

        try
        {
            await Shell.Current.GoToAsync(
                $"colaborador?Id={colaborador.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao navegar para tela de edição: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadColaboradoresAsync();
    }

    [RelayCommand]
    private async Task SearchColaboradoresAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await MainThread.InvokeOnMainThreadAsync(
                () => Colaboradores.Clear());

            IEnumerable<ColaboradorDto> resultados =
                Enumerable.Empty<ColaboradorDto>();

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                resultados =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? Enumerable.Empty<ColaboradorDto>();
            }
            else if (SelectedFilterType == "Nome")
            {
                var todos =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? Enumerable.Empty<ColaboradorDto>();

                string termo = SearchText.Trim();

                resultados = todos.Where(c =>
                    c.Nome != null &&
                    c.Nome.Contains(
                        termo,
                        StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedFilterType == "CPF")
            {
                string cpfLimpo = new(
                    SearchText.Where(char.IsDigit).ToArray());

                if (cpfLimpo.Length != 11)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Para buscar por CPF, informe os 11 dígitos numéricos.",
                        "OK");
                    return;
                }

                var colaborador =
                    await _colaboradorService.ObterPorCpfAsync(
                        cpfLimpo, cts.Token);

                if (colaborador != null)
                    resultados = new[] { colaborador };
            }
            else if (SelectedFilterType == "Id")
            {
                if (!int.TryParse(
                        SearchText.Trim(), out int id) || id <= 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Para buscar por ID, informe um número inteiro positivo válido.",
                        "OK");
                    return;
                }

                var colaborador =
                    await _colaboradorService.ObterPorIdAsync(
                        id, cts.Token);

                if (colaborador != null)
                    resultados = new[] { colaborador };
            }
            else if (SelectedFilterType == "Email")
            {
                var colaborador =
                    await _colaboradorService.ObterPorEmailAsync(
                        SearchText.Trim(), cts.Token);

                if (colaborador != null)
                    resultados = new[] { colaborador };
            }
            else if (SelectedFilterType == "Tipo")
            {
                string termo = SearchText.Trim();

                var correspondentes =
                    Enum.GetValues<EColaboradorTipo>()
                        .Where(t =>
                            t.ToString().Contains(
                                termo,
                                StringComparison.OrdinalIgnoreCase) ||
                            t.GetDisplayName().Contains(
                                termo,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (correspondentes.Count == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe um tipo válido, como Administrador, Atendente, Instrutor ou Professor.",
                        "OK");
                    return;
                }

                var listaPorTipo = new List<ColaboradorDto>();

                foreach (var tipo in correspondentes)
                {
                    var itens =
                        await _colaboradorService.ObterPorTipoAsync(
                            tipo, cts.Token);

                    if (itens != null)
                        listaPorTipo.AddRange(itens);
                }

                resultados = listaPorTipo.DistinctBy(c => c.Id);
            }
            else if (SelectedFilterType == "Vínculo")
            {
                string termo = SearchText.Trim();

                var correspondentes =
                    Enum.GetValues<EColaboradorVinculo>()
                        .Where(v =>
                            v.ToString().Contains(
                                termo,
                                StringComparison.OrdinalIgnoreCase) ||
                            v.GetDisplayName().Contains(
                                termo,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (correspondentes.Count == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe um vínculo válido, como CLT ou Estágio.",
                        "OK");
                    return;
                }

                var todos =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? Enumerable.Empty<ColaboradorDto>();

                resultados = todos.Where(c =>
                    correspondentes.Any(v =>
                        string.Equals(
                            c.Vinculo.ToString(),
                            v.ToString(),
                            StringComparison.OrdinalIgnoreCase)));
            }

            var listaFinal = resultados.ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (var item in listaFinal)
                    Colaboradores.Add(item);

                OnPropertyChanged(nameof(Colaboradores));
            });
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "A busca de colaboradores expirou. Verifique o banco de dados.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao buscar colaboradores: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadColaboradoresAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await MainThread.InvokeOnMainThreadAsync(
                () => Colaboradores.Clear());

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var colaboradoresList =
                await _colaboradorService.ObterTodosAsync(cts.Token);

            if (colaboradoresList != null)
            {
                var lista = colaboradoresList.ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var colaborador in lista)
                        Colaboradores.Add(colaborador);

                    OnPropertyChanged(nameof(Colaboradores));
                });
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "O carregamento dos colaboradores expirou. Verifique o banco de dados.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Erro ao carregar colaboradores: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteColaboradorAsync(
        ColaboradorDto colaborador)
    {
        if (colaborador == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar exclusão",
            $"Deseja realmente excluir o colaborador {colaborador.Nome}?",
            "Sim",
            "Não");

        if (!confirm)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            bool success =
                await _colaboradorService.RemoverAsync(
                    colaborador.Id, cts.Token);

            if (success)
            {
                Colaboradores.Remove(colaborador);

                await Shell.Current.DisplayAlert(
                    "Sucesso",
                    "Colaborador excluído com sucesso!",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Erro",
                    "Não foi possível excluir o colaborador.",
                    "OK");
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo esgotado",
                "A exclusão do colaborador expirou. Verifique o banco de dados.",
                "OK");
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains(
                    "FOREIGN KEY",
                    StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains(
                    "constraint",
                    StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains(
                    "REFERENCE",
                    StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert(
                    "Não permitido",
                    "Este colaborador não pode ser excluído porque possui registros vinculados no sistema.",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Erro",
                    $"Erro ao excluir colaborador: {ex.Message}",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
