
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaListViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;

    public ObservableCollection<string> FilterTypes { get; } =
    [
        "Id",
        "Id Aluno",
        "Ativas",
        "Vencendo em dias"
    ];

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Ativas";

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    private ObservableCollection<MatriculaDto> _matriculas = [];

    public ObservableCollection<MatriculaDto> Matriculas
    {
        get => _matriculas;
        set => SetProperty(ref _matriculas, value);
    }

    private MatriculaDto? _selectedMatricula;

    public MatriculaDto? SelectedMatricula
    {
        get => _selectedMatricula;
        set => SetProperty(ref _selectedMatricula, value);
    }

    public MatriculaListViewModel(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
        Title = "Matrículas";
    }

    [RelayCommand]
    private async Task AddMatriculaAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("matricula");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao navegar para tela de cadastro: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task EditMatriculaAsync(MatriculaDto matricula)
    {
        if (matricula == null)
            return;

        try
        {
            await Shell.Current.GoToAsync(
                $"matricula?Id={matricula.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao navegar para tela de edição: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsRefreshing = true;

        try
        {
            await LoadMatriculasAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task SearchMatriculasAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            IEnumerable<MatriculaDto> resultados = [];

            string texto = SearchText?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(texto))
            {
                if (SelectedFilterType == "Ativas")
                {
                    resultados =
                        await _matriculaService.ObterAtivasAsync(
                            0,
                            cts.Token) ?? [];
                }
                else
                {
                    resultados =
                        await _matriculaService.ObterTodasAsync(
                            cts.Token) ?? [];
                }
            }
            else if (SelectedFilterType == "Id")
            {
                if (int.TryParse(texto, out int id))
                {
                    var matricula =
                        await _matriculaService.ObterPorIdAsync(
                            id,
                            cts.Token);

                    if (matricula != null)
                        resultados = [matricula];
                }
                else
                {
                    await ExibirAvisoAsync(
                        "Para busca por ID, informe um número válido.");
                }
            }
            else if (SelectedFilterType == "Id Aluno")
            {
                if (int.TryParse(texto, out int alunoId))
                {
                    resultados =
                        await _matriculaService.ObterPorAlunoIdAsync(
                            alunoId,
                            cts.Token) ?? [];
                }
                else
                {
                    await ExibirAvisoAsync(
                        "Para busca por ID do Aluno, informe um número válido.");
                }
            }
            else if (SelectedFilterType == "Ativas")
            {
                if (int.TryParse(texto, out int alunoId))
                {
                    resultados =
                        await _matriculaService.ObterAtivasAsync(
                            alunoId,
                            cts.Token) ?? [];
                }
                else
                {
                    await ExibirAvisoAsync(
                        "Informe um ID de aluno válido.");
                }
            }
            else if (SelectedFilterType == "Vencendo em dias")
            {
                if (int.TryParse(texto, out int dias) && dias >= 0)
                {
                    resultados =
                        await _matriculaService.ObterVencendoEmDiasAsync(
                            dias,
                            cts.Token) ?? [];
                }
                else
                {
                    await ExibirAvisoAsync(
                        "Informe uma quantidade de dias válida.");
                }
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Matriculas.Clear();

                foreach (var item in resultados)
                    Matriculas.Add(item);
            });
        }
        catch (OperationCanceledException)
        {
            await ExibirErroAsync(
                "A busca demorou muito e foi cancelada. Tente novamente.");
        }
        catch (Exception ex)
        {
            await ExibirErroAsync(
                $"Erro ao buscar matrículas: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadMatriculasAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var matriculasList =
                await _matriculaService.ObterTodasAsync(cts.Token);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Matriculas.Clear();

                if (matriculasList == null)
                    return;

                foreach (var matricula in matriculasList)
                    Matriculas.Add(matricula);
            });
        }
        catch (OperationCanceledException)
        {
            await ExibirErroAsync(
                "O carregamento demorou muito e foi cancelado.");
        }
        catch (Exception ex)
        {
            await ExibirErroAsync(
                $"Erro ao carregar matrículas: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteMatriculaAsync(MatriculaDto matricula)
    {
        if (matricula == null || IsBusy)
            return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Deseja realmente excluir a matrícula {matricula.Id}?",
            "Sim",
            "Não");

        if (!confirm)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            bool success = await _matriculaService.RemoverAsync(
                matricula.Id,
                cts.Token);

            if (success)
            {
                await MainThread.InvokeOnMainThreadAsync(
                    () => Matriculas.Remove(matricula));

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Matrícula excluída com sucesso!",
                    "OK");
            }
            else
            {
                await ExibirErroAsync(
                    "Não foi possível excluir a matrícula.");
            }
        }
        catch (OperationCanceledException)
        {
            await ExibirErroAsync(
                "A exclusão demorou muito e foi cancelada.");
        }
        catch (Exception ex)
        {
            await ExibirErroAsync(
                $"Erro ao excluir matrícula: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static Task ExibirAvisoAsync(string mensagem)
    {
        return Shell.Current.DisplayAlertAsync(
            "Aviso",
            mensagem,
            "OK");
    }

    private static Task ExibirErroAsync(string mensagem)
    {
        return Shell.Current.DisplayAlertAsync(
            "Erro",
            mensagem,
            "OK");
    }
}
