
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
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

    private static string ObterNomeExibicao(Enum valor)
    {
        var membro = valor.GetType().GetField(valor.ToString());

        var atributo = membro?.GetCustomAttribute<DisplayAttribute>();

        return atributo?.GetName() ?? valor.ToString();
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
                $"Erro ao abrir cadastro: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task EditColaboradorAsync(ColaboradorDto colaborador)
    {
        if (colaborador is null)
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
                $"Erro ao abrir edição: {ex.Message}",
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

            IEnumerable<ColaboradorDto> resultados = [];

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                resultados =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? [];
            }
            else if (SelectedFilterType == "Nome")
            {
                var todos =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? [];

                string termo = SearchText.Trim();

                resultados = todos.Where(c =>
                    (c.Nome ?? string.Empty).Contains(
                        termo,
                        StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedFilterType == "CPF")
            {
                string cpfLimpo =
                    new(SearchText.Where(char.IsDigit).ToArray());

                if (cpfLimpo.Length != 11)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe os 11 dígitos do CPF.",
                        "OK");
                    return;
                }

                var colaborador =
                    await _colaboradorService.ObterPorCpfAsync(
                        cpfLimpo, cts.Token);

                if (colaborador is not null)
                    resultados = [colaborador];
            }
            else if (SelectedFilterType == "Id")
            {
                if (!int.TryParse(SearchText.Trim(), out int id) || id <= 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe um ID inteiro positivo.",
                        "OK");
                    return;
                }

                var colaborador =
                    await _colaboradorService.ObterPorIdAsync(
                        id, cts.Token);

                if (colaborador is not null)
                    resultados = [colaborador];
            }
            else if (SelectedFilterType == "Email")
            {
                var colaborador =
                    await _colaboradorService.ObterPorEmailAsync(
                        SearchText.Trim(), cts.Token);

                if (colaborador is not null)
                    resultados = [colaborador];
            }
            else if (SelectedFilterType == "Tipo")
            {
                string termo = SearchText.Trim();

                var tipos = Enum.GetValues<EColaboradorTipo>()
                    .Where(t =>
                        t.ToString().Contains(
                            termo, StringComparison.OrdinalIgnoreCase)
                        || ObterNomeExibicao(t).Contains(
                            termo, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (tipos.Count == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe Administrador, Atendente ou Instrutor.",
                        "OK");
                    return;
                }

                var todos =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? [];

                resultados = todos.Where(c => tipos.Contains(c.Tipo));
            }
            else if (SelectedFilterType == "Vínculo")
            {
                string termo = SearchText.Trim();

                var vinculos = Enum.GetValues<EColaboradorVinculo>()
                    .Where(v =>
                        v.ToString().Contains(
                            termo, StringComparison.OrdinalIgnoreCase)
                        || ObterNomeExibicao(v).Contains(
                            termo, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (vinculos.Count == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Validação",
                        "Informe um vínculo válido, como CLT ou Estágio.",
                        "OK");
                    return;
                }

                var todos =
                    await _colaboradorService.ObterTodosAsync(cts.Token)
                    ?? [];

                resultados = todos.Where(c => vinculos.Contains(c.Vinculo));
            }

            var listaResultados = resultados.ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Colaboradores.Clear();

                foreach (var item in listaResultados)
                    Colaboradores.Add(item);
            });
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "A busca expirou. Verifique a conexão com o banco.",
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
    public async Task LoadColaboradoresAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var lista =
                await _colaboradorService.ObterTodosAsync(cts.Token);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Colaboradores.Clear();

                if (lista is not null)
                {
                    foreach (var colaborador in lista)
                        Colaboradores.Add(colaborador);
                }
            });
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlert(
                "Tempo Esgotado",
                "O carregamento expirou. Verifique a conexão.",
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
    private async Task DeleteColaboradorAsync(ColaboradorDto colaborador)
    {
        if (colaborador is null || IsBusy)
            return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar Exclusão",
            $"Deseja realmente excluir o colaborador {colaborador.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            bool sucesso = await _colaboradorService.RemoverAsync(
                colaborador.Id,
                cts.Token);

            if (sucesso)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var existente = Colaboradores.FirstOrDefault(
                        c => c.Id == colaborador.Id);

                    if (existente is not null)
                        Colaboradores.Remove(existente);

                    if (SelectedColaborador?.Id == colaborador.Id)
                        SelectedColaborador = null;
                });

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
                "Tempo Esgotado",
                "A exclusão expirou. Verifique a conexão.",
                "OK");
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains(
                    "FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains(
                    "constraint", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains(
                    "REFERENCE", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert(
                    "Não Permitido",
                    "Este colaborador possui registros vinculados e não pode ser excluído.",
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
