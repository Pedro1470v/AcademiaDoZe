
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoListViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;

    public ObservableCollection<AlunoDto> Alunos { get; } = [];

    public ObservableCollection<string> FilterTypes { get; } =
        ["Nome", "Id", "CPF", "Email"];

    private AlunoDto? _selectedAluno;

    public AlunoDto? SelectedAluno
    {
        get => _selectedAluno;
        set => SetProperty(ref _selectedAluno, value);
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

    public IAsyncRelayCommand LoadAlunosCommand { get; }

    public AlunoListViewModel(IAlunoService alunoService)
    {
        _alunoService = alunoService;
        Title = "Alunos";

        LoadAlunosCommand = new AsyncRelayCommand(LoadAlunosAsync);
    }

    public async Task LoadAlunosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            var alunos = await _alunoService.ObterTodosAsync(cts.Token);

            Alunos.Clear();

            foreach (var aluno in alunos)
                Alunos.Add(aluno);
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "O carregamento dos alunos demorou demais.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível carregar os alunos: {ex.Message}",
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
        await LoadAlunosAsync();
    }

    [RelayCommand]
    private async Task AddAlunoAsync()
    {
        await Shell.Current.GoToAsync("aluno");
    }

    [RelayCommand]
    private async Task EditAlunoAsync(AlunoDto aluno)
    {
        if (aluno is null)
            return;

        await Shell.Current.GoToAsync($"aluno?Id={aluno.Id}");
    }

    [RelayCommand]
    private async Task DeleteAlunoAsync(AlunoDto aluno)
    {
        if (aluno is null)
            return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar exclusão",
            $"Deseja realmente excluir o aluno {aluno.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            bool removido = await _alunoService.RemoverAsync(
                aluno.Id,
                cts.Token);

            if (removido)
            {
                Alunos.Remove(aluno);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Aluno excluído com sucesso!",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "O aluno não foi encontrado ou não pôde ser excluído.",
                    "OK");
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "A exclusão demorou demais.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível excluir o aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchAlunosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            IEnumerable<AlunoDto> resultados = [];

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                resultados = await _alunoService.ObterTodosAsync(
                    cts.Token);
            }
            else
            {
                string pesquisa = SearchText.Trim();

                switch (SelectedFilterType)
                {
                    case "Nome":
                        resultados = await _alunoService.ObterPorNomeAsync(
                            pesquisa,
                            cts.Token);
                        break;

                    case "Id":
                        if (int.TryParse(pesquisa, out int id) && id > 0)
                        {
                            var aluno = await _alunoService.ObterPorIdAsync(
                                id,
                                cts.Token);

                            if (aluno is not null)
                                resultados = [aluno];
                        }
                        break;

                    case "CPF":
                        var cpf = new string(
                            pesquisa.Where(char.IsDigit).ToArray());

                        if (cpf.Length == 11)
                        {
                            var aluno = await _alunoService.ObterPorCpfAsync(
                                cpf,
                                cts.Token);

                            if (aluno is not null)
                                resultados = [aluno];
                        }
                        break;

                    case "Email":
                        var alunoEmail = await _alunoService.ObterPorEmailAsync(
                            pesquisa,
                            cts.Token);

                        if (alunoEmail is not null)
                            resultados = [alunoEmail];
                        break;
                }
            }

            Alunos.Clear();

            foreach (var aluno in resultados)
                Alunos.Add(aluno);
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "A pesquisa demorou demais.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível pesquisar os alunos: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
