
// Pedro Vinicius

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public class ControleAcessoViewModel : INotifyPropertyChanged
{
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;
    private readonly AcessoAlunoService _acessoAlunoService;
    private readonly AcessoColaboradorService _acessoColaboradorService;

    private string _tipoSelecionado = "Aluno";
    private string _textoPesquisa = string.Empty;
    private PessoaAcessoItem? _pessoaSelecionada;

    private string _nomePessoa = "Nome da pessoa";
    private string _cpfPessoa = "CPF: -";
    private string _planoVinculo = "Plano / Vínculo: -";
    private string _statusPessoa = "Status: Não selecionado";
    private string _resultadoAcesso = "Aguardando identificação...";
    private string _tituloResultado = "Resultado do acesso";
    private string _textoBotaoAcesso = "Registrar Entrada";

    private ImageSource? _fotoPessoa;

    private Color _corStatus = Colors.Gray;
    private bool _isBusy;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<PessoaAcessoItem> PessoasDisponiveis { get; } = new();

    public ObservableCollection<AcessoHistoricoItem> HistoricoAcessos { get; } = new();

    public ControleAcessoViewModel(
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService,
        AcessoAlunoService acessoAlunoService,
        AcessoColaboradorService acessoColaboradorService)
    {
        _alunoService = alunoService
            ?? throw new ArgumentNullException(nameof(alunoService));

        _colaboradorService = colaboradorService
            ?? throw new ArgumentNullException(nameof(colaboradorService));

        _matriculaService = matriculaService
            ?? throw new ArgumentNullException(nameof(matriculaService));

        _acessoAlunoService = acessoAlunoService
            ?? throw new ArgumentNullException(nameof(acessoAlunoService));

        _acessoColaboradorService = acessoColaboradorService
            ?? throw new ArgumentNullException(nameof(acessoColaboradorService));

        SelecionarAlunoCommand = new Command(SelecionarAluno);
        SelecionarColaboradorCommand = new Command(SelecionarColaborador);

        PesquisarCommand = new Command(async () => await PesquisarPessoaAsync());
        RegistrarAcessoCommand = new Command(async () => await RegistrarAcessoAsync());
        AtualizarHistoricoCommand = new Command(async () => await AtualizarHistoricoAsync());
    }

    public Command SelecionarAlunoCommand { get; }
    public Command SelecionarColaboradorCommand { get; }
    public Command PesquisarCommand { get; }
    public Command RegistrarAcessoCommand { get; }
    public Command AtualizarHistoricoCommand { get; }

    public string TipoSelecionado
    {
        get => _tipoSelecionado;
        private set
        {
            if (!SetProperty(ref _tipoSelecionado, value))
                return;

            OnPropertyChanged(nameof(CorBotaoAluno));
            OnPropertyChanged(nameof(CorBotaoColaborador));
            OnPropertyChanged(nameof(ModoSelecionadoTexto));
            OnPropertyChanged(nameof(TextoSelecaoPessoa));
        }
    }

    public string TextoPesquisa
    {
        get => _textoPesquisa;
        set => SetProperty(ref _textoPesquisa, value);
    }

    public PessoaAcessoItem? PessoaSelecionada
    {
        get => _pessoaSelecionada;
        set
        {
            if (!SetProperty(ref _pessoaSelecionada, value))
                return;

            if (value == null)
            {
                LimparIdentificacao();
                return;
            }

            NomePessoa = value.Nome;
            CpfPessoa = $"CPF: {value.Cpf}";
            PlanoVinculo = value.DescricaoVinculo;

            // Carrega a foto cadastrada da pessoa.
            FotoPessoa = CarregarFoto(value.FotoConteudo);

            StatusPessoa = "Status: Cadastro localizado";
            CorStatus = Colors.LightGreen;

            TituloResultado = "Pessoa identificada";
            ResultadoAcesso = $"{value.Nome} foi localizada no cadastro.";

            _ = AtualizarBotaoAcessoAsync(value);
        }
    }

    public string NomePessoa
    {
        get => _nomePessoa;
        set => SetProperty(ref _nomePessoa, value);
    }

    public string CpfPessoa
    {
        get => _cpfPessoa;
        set => SetProperty(ref _cpfPessoa, value);
    }

    public string PlanoVinculo
    {
        get => _planoVinculo;
        set => SetProperty(ref _planoVinculo, value);
    }

    public string StatusPessoa
    {
        get => _statusPessoa;
        set => SetProperty(ref _statusPessoa, value);
    }

    public string ResultadoAcesso
    {
        get => _resultadoAcesso;
        set => SetProperty(ref _resultadoAcesso, value);
    }

    public string TituloResultado
    {
        get => _tituloResultado;
        set => SetProperty(ref _tituloResultado, value);
    }

    public string TextoBotaoAcesso
    {
        get => _textoBotaoAcesso;
        set => SetProperty(ref _textoBotaoAcesso, value);
    }

    // Imagem que será utilizada no Image do XAML.
    public ImageSource? FotoPessoa
    {
        get => _fotoPessoa;
        set => SetProperty(ref _fotoPessoa, value);
    }

    public Color CorStatus
    {
        get => _corStatus;
        set => SetProperty(ref _corStatus, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public Color CorBotaoAluno =>
        TipoSelecionado == "Aluno"
            ? Color.FromArgb("#A98AEF")
            : Color.FromArgb("#512BD4");

    public Color CorBotaoColaborador =>
        TipoSelecionado == "Colaborador"
            ? Color.FromArgb("#A98AEF")
            : Color.FromArgb("#512BD4");

    public string ModoSelecionadoTexto =>
        $"Modo atual selecionado: {TipoSelecionado}";

    public string TextoSelecaoPessoa =>
        TipoSelecionado == "Aluno"
            ? "Selecione o Aluno:"
            : "Selecione o Colaborador:";

    // Converte os bytes da foto em uma imagem para o MAUI.
    private ImageSource? CarregarFoto(byte[]? conteudo)
    {
        if (conteudo == null || conteudo.Length == 0)
            return null;

        byte[] copiaConteudo = conteudo.ToArray();

        return ImageSource.FromStream(
            () => new MemoryStream(copiaConteudo));
    }

    private void SelecionarAluno()
    {
        TipoSelecionado = "Aluno";
        LimparPesquisaESelecao();

        TituloResultado = "Modo Aluno selecionado";
        ResultadoAcesso = "Informe o CPF, ID ou nome do aluno para pesquisar.";
    }

    private void SelecionarColaborador()
    {
        TipoSelecionado = "Colaborador";
        LimparPesquisaESelecao();

        TituloResultado = "Modo Colaborador selecionado";
        ResultadoAcesso = "Informe o CPF, ID ou nome do colaborador para pesquisar.";
    }

    private async Task PesquisarPessoaAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            TituloResultado = "Pesquisa não realizada";
            ResultadoAcesso = "Digite o CPF, ID ou nome da pessoa.";
            return;
        }

        try
        {
            IsBusy = true;

            PessoasDisponiveis.Clear();
            PessoaSelecionada = null;

            string pesquisa = TextoPesquisa.Trim();
            string cpfNumerico = new string(pesquisa.Where(char.IsDigit).ToArray());

            if (TipoSelecionado == "Aluno")
                await PesquisarAlunoAsync(pesquisa, cpfNumerico);
            else
                await PesquisarColaboradorAsync(pesquisa, cpfNumerico);

            if (PessoasDisponiveis.Count == 0)
            {
                TituloResultado = "Pessoa não encontrada";
                ResultadoAcesso = $"Nenhum {TipoSelecionado.ToLower()} foi encontrado.";
                StatusPessoa = "Status: Não encontrado";
                CorStatus = Colors.OrangeRed;
                return;
            }

            TituloResultado = "Pesquisa concluída";
            ResultadoAcesso = $"{PessoasDisponiveis.Count} pessoa(s) encontrada(s).";

            if (PessoasDisponiveis.Count == 1)
                PessoaSelecionada = PessoasDisponiveis[0];
        }
        catch (Exception ex)
        {
            TituloResultado = "Erro na pesquisa";
            ResultadoAcesso = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PesquisarAlunoAsync(string pesquisa, string cpfNumerico)
    {
        if (cpfNumerico.Length == 11)
        {
            var aluno = await _alunoService.ObterPorCpfAsync(pesquisa);

            if (aluno == null && cpfNumerico != pesquisa)
                aluno = await _alunoService.ObterPorCpfAsync(cpfNumerico);

            if (aluno != null)
            {
                AdicionarAlunoNaLista(aluno);
                return;
            }
        }

        if (int.TryParse(pesquisa, out int id))
        {
            var aluno = await _alunoService.ObterPorIdAsync(id);

            if (aluno != null)
            {
                AdicionarAlunoNaLista(aluno);
                return;
            }
        }

        var alunos = await _alunoService.ObterPorNomeAsync(pesquisa);

        foreach (var aluno in alunos)
            AdicionarAlunoNaLista(aluno);
    }

    private async Task PesquisarColaboradorAsync(string pesquisa, string cpfNumerico)
    {
        if (cpfNumerico.Length == 11)
        {
            var colaborador = await _colaboradorService.ObterPorCpfAsync(pesquisa);

            if (colaborador == null && cpfNumerico != pesquisa)
                colaborador = await _colaboradorService.ObterPorCpfAsync(cpfNumerico);

            if (colaborador != null)
            {
                AdicionarColaboradorNaLista(colaborador);
                return;
            }
        }

        if (int.TryParse(pesquisa, out int id))
        {
            var colaborador = await _colaboradorService.ObterPorIdAsync(id);

            if (colaborador != null)
            {
                AdicionarColaboradorNaLista(colaborador);
                return;
            }
        }

        var colaboradores = await _colaboradorService.ObterPorNomeAsync(pesquisa);

        foreach (var colaborador in colaboradores)
            AdicionarColaboradorNaLista(colaborador);
    }

    private void AdicionarAlunoNaLista(AlunoDto aluno)
    {
        if (PessoasDisponiveis.Any(p => p.Id == aluno.Id && p.Tipo == "Aluno"))
            return;

        PessoasDisponiveis.Add(new PessoaAcessoItem
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf,
            Tipo = "Aluno",
            Descricao = $"{aluno.Nome} - CPF: {aluno.Cpf}",
            DescricaoVinculo = "Tipo: Aluno",

            // Foto herdada de PessoaDto.
            FotoConteudo = aluno.Foto?.Conteudo
        });
    }

    private void AdicionarColaboradorNaLista(ColaboradorDto colaborador)
    {
        if (PessoasDisponiveis.Any(p => p.Id == colaborador.Id && p.Tipo == "Colaborador"))
            return;

        PessoasDisponiveis.Add(new PessoaAcessoItem
        {
            Id = colaborador.Id,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf,
            Tipo = "Colaborador",
            Descricao = $"{colaborador.Nome} - CPF: {colaborador.Cpf}",
            DescricaoVinculo = $"Vínculo: {colaborador.Vinculo} | Tipo: {colaborador.Tipo}",

            // Foto cadastrada no DTO do colaborador.
            FotoConteudo = colaborador.Foto?.Conteudo
        });
    }

    private async Task AtualizarInformacaoPlanoAsync(int alunoId)
    {
        try
        {
            var matricula = await _matriculaService
                .ObterMatriculaAtivaPorAlunoAsync(alunoId);

            if (matricula == null)
            {
                PlanoVinculo = "Aluno sem matrícula ativa.";
                return;
            }

            DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

            int diasRestantes = matricula.DataFim.DayNumber - hoje.DayNumber;
            string nomePlano = matricula.Plano.ToString();

            if (diasRestantes < 0)
            {
                PlanoVinculo =
                    $"Plano: {nomePlano} | Vencido há {Math.Abs(diasRestantes)} dia(s).";
            }
            else if (diasRestantes == 0)
            {
                PlanoVinculo = $"Plano: {nomePlano} | Vence hoje.";
            }
            else
            {
                PlanoVinculo =
                    $"Plano: {nomePlano} | Restam {diasRestantes} dia(s).";
            }
        }
        catch (Exception ex)
        {
            PlanoVinculo = "Não foi possível consultar o plano.";

            System.Diagnostics.Debug.WriteLine(
                $"Erro ao consultar matrícula do aluno: {ex.Message}");
        }
    }

    private async Task AtualizarBotaoAcessoAsync(PessoaAcessoItem pessoa)
    {
        try
        {
            if (pessoa.Tipo == "Aluno")
            {
                await AtualizarInformacaoPlanoAsync(pessoa.Id);

                var acessos = await _acessoAlunoService.ObterTodosAsync();

                var acessoAberto = acessos
                    .Where(a => a.Aluno.ID == pessoa.Id && a.DataHoraSaida == null)
                    .OrderByDescending(a => a.DataHoraEntrada)
                    .FirstOrDefault();

                TextoBotaoAcesso = acessoAberto == null
                    ? "Registrar Entrada"
                    : "Registrar Saída";

                StatusPessoa = acessoAberto == null
                    ? "Status: Cadastro localizado"
                    : "Status: Dentro da academia";
            }
            else
            {
                PlanoVinculo = pessoa.DescricaoVinculo;

                var acessos = await _acessoColaboradorService.ObterTodosAsync();

                var acessoAberto = acessos
                    .Where(a => a.Colaborador.ID == pessoa.Id && a.DataHoraSaida == null)
                    .OrderByDescending(a => a.DataHoraEntrada)
                    .FirstOrDefault();

                TextoBotaoAcesso = acessoAberto == null
                    ? "Registrar Entrada"
                    : "Registrar Saída";

                StatusPessoa = acessoAberto == null
                    ? "Status: Cadastro localizado"
                    : "Status: Dentro da academia";
            }

            CorStatus = Colors.LightGreen;
        }
        catch (Exception ex)
        {
            TituloResultado = "Erro ao consultar acesso";
            ResultadoAcesso = ex.Message;
        }
    }

    private async Task RegistrarAcessoAsync()
    {
        if (IsBusy)
            return;

        if (PessoaSelecionada == null)
        {
            TituloResultado = "Acesso não registrado";
            ResultadoAcesso = "Pesquise e selecione uma pessoa primeiro.";
            return;
        }

        try
        {
            IsBusy = true;

            if (PessoaSelecionada.Tipo == "Aluno")
                await RegistrarAcessoAlunoAsync(PessoaSelecionada);
            else
                await RegistrarAcessoColaboradorAsync(PessoaSelecionada);

            await AtualizarHistoricoAsync();
        }
        catch (Exception ex)
        {
            TituloResultado = "Não foi possível registrar o acesso";
            ResultadoAcesso = ex.Message;

            if (PessoaSelecionada != null)
                await AtualizarBotaoAcessoAsync(PessoaSelecionada);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RegistrarAcessoAlunoAsync(PessoaAcessoItem pessoa)
    {
        var acessos = await _acessoAlunoService.ObterTodosAsync();

        var acessoAberto = acessos
            .Where(a => a.Aluno.ID == pessoa.Id && a.DataHoraSaida == null)
            .OrderByDescending(a => a.DataHoraEntrada)
            .FirstOrDefault();

        if (acessoAberto == null)
        {
            var novoAcesso = await _acessoAlunoService.RegistrarEntradaAsync(
                pessoa.Id,
                DateTime.Now);

            TextoBotaoAcesso = "Registrar Saída";
            StatusPessoa = "Status: Dentro da academia";
            CorStatus = Colors.LightGreen;
            TituloResultado = "Entrada registrada";

            ResultadoAcesso =
                $"{pessoa.Nome} teve a entrada registrada às {novoAcesso.DataHoraEntrada:HH:mm:ss}.";
        }
        else
        {
            DateTime horarioSaida = DateTime.Now;

            var acessoAtualizado = await _acessoAlunoService.RegistrarSaidaAsync(
                acessoAberto.ID,
                horarioSaida);

            TimeSpan permanencia =
                acessoAtualizado.DataHoraSaida!.Value -
                acessoAtualizado.DataHoraEntrada;

            TextoBotaoAcesso = "Registrar Entrada";
            StatusPessoa = "Status: Fora da academia";
            CorStatus = Colors.Gray;
            TituloResultado = "Saída registrada";

            ResultadoAcesso =
                $"{pessoa.Nome} saiu às {horarioSaida:HH:mm:ss}. Tempo de permanência: {permanencia.Hours}h {permanencia.Minutes}min.";
        }

        await AtualizarInformacaoPlanoAsync(pessoa.Id);
    }

    private async Task RegistrarAcessoColaboradorAsync(PessoaAcessoItem pessoa)
    {
        var acessos = await _acessoColaboradorService.ObterTodosAsync();

        var acessoAberto = acessos
            .Where(a => a.Colaborador.ID == pessoa.Id && a.DataHoraSaida == null)
            .OrderByDescending(a => a.DataHoraEntrada)
            .FirstOrDefault();

        if (acessoAberto == null)
        {
            var novoAcesso = await _acessoColaboradorService.RegistrarEntradaAsync(
                pessoa.Id,
                DateTime.Now);

            TextoBotaoAcesso = "Registrar Saída";
            StatusPessoa = "Status: Dentro da academia";
            CorStatus = Colors.LightGreen;
            TituloResultado = "Entrada de colaborador registrada";

            ResultadoAcesso =
                $"{pessoa.Nome} teve a entrada registrada às {novoAcesso.DataHoraEntrada:HH:mm:ss}.";
        }
        else
        {
            DateTime horarioSaida = DateTime.Now;

            var acessoAtualizado = await _acessoColaboradorService.RegistrarSaidaAsync(
                acessoAberto.ID,
                horarioSaida);

            TimeSpan permanencia =
                acessoAtualizado.DataHoraSaida!.Value -
                acessoAtualizado.DataHoraEntrada;

            TextoBotaoAcesso = "Registrar Entrada";
            StatusPessoa = "Status: Fora da academia";
            CorStatus = Colors.Gray;
            TituloResultado = "Saída de colaborador registrada";

            ResultadoAcesso =
                $"{pessoa.Nome} saiu às {horarioSaida:HH:mm:ss}. Tempo de permanência: {permanencia.Hours}h {permanencia.Minutes}min.";
        }
    }

    private async Task AtualizarHistoricoAsync()
    {
        try
        {
            var acessosAlunos = await _acessoAlunoService.ObterTodosAsync();
            var acessosColaboradores = await _acessoColaboradorService.ObterTodosAsync();

            HistoricoAcessos.Clear();

            foreach (var acesso in acessosAlunos
                .Where(a => a.DataHoraEntrada.Date == DateTime.Today))
            {
                string descricao;

                if (acesso.DataHoraSaida.HasValue)
                {
                    TimeSpan permanencia =
                        acesso.DataHoraSaida.Value - acesso.DataHoraEntrada;

                    descricao =
                        $"Entrada: {acesso.DataHoraEntrada:HH:mm} | Saída: {acesso.DataHoraSaida:HH:mm} | Permanência: {permanencia.Hours}h {permanencia.Minutes}min";
                }
                else
                {
                    descricao =
                        $"Entrada: {acesso.DataHoraEntrada:HH:mm} | Acesso em andamento";
                }

                HistoricoAcessos.Add(new AcessoHistoricoItem
                {
                    Nome = $"{acesso.Aluno.Nome} (Aluno)",
                    Descricao = descricao,
                    Horario = acesso.DataHoraEntrada.ToString("HH:mm")
                });
            }

            foreach (var acesso in acessosColaboradores
                .Where(a => a.DataHoraEntrada.Date == DateTime.Today))
            {
                string descricao;

                if (acesso.DataHoraSaida.HasValue)
                {
                    TimeSpan permanencia =
                        acesso.DataHoraSaida.Value - acesso.DataHoraEntrada;

                    descricao =
                        $"Entrada: {acesso.DataHoraEntrada:HH:mm} | Saída: {acesso.DataHoraSaida:HH:mm} | Permanência: {permanencia.Hours}h {permanencia.Minutes}min";
                }
                else
                {
                    descricao =
                        $"Entrada: {acesso.DataHoraEntrada:HH:mm} | Acesso em andamento";
                }

                HistoricoAcessos.Add(new AcessoHistoricoItem
                {
                    Nome = $"{acesso.Colaborador.Nome} (Colaborador)",
                    Descricao = descricao,
                    Horario = acesso.DataHoraEntrada.ToString("HH:mm")
                });
            }

            var historicoOrdenado = HistoricoAcessos
                .OrderByDescending(h => h.Horario)
                .ToList();

            HistoricoAcessos.Clear();

            foreach (var item in historicoOrdenado)
                HistoricoAcessos.Add(item);

            if (HistoricoAcessos.Count == 0)
            {
                TituloResultado = "Histórico atualizado";
                ResultadoAcesso = "Nenhum acesso registrado hoje.";
            }
        }
        catch (Exception ex)
        {
            TituloResultado = "Erro ao atualizar histórico";
            ResultadoAcesso = ex.Message;
        }
    }

    private void LimparPesquisaESelecao()
    {
        TextoPesquisa = string.Empty;
        PessoasDisponiveis.Clear();
        PessoaSelecionada = null;
        TextoBotaoAcesso = "Registrar Entrada";
        LimparIdentificacao();
    }

    private void LimparIdentificacao()
    {
        NomePessoa = "Nome da pessoa";
        CpfPessoa = "CPF: -";
        PlanoVinculo = "Plano / Vínculo: -";
        StatusPessoa = "Status: Não selecionado";
        CorStatus = Colors.Gray;
        TextoBotaoAcesso = "Registrar Entrada";

        // Limpa a foto ao desmarcar a pessoa.
        FotoPessoa = null;
    }

    private bool SetProperty<T>(
        ref T campo,
        T valor,
        [CallerMemberName] string? nomePropriedade = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(nomePropriedade);
        return true;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? nomePropriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nomePropriedade));
    }
}

public class PessoaAcessoItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string DescricaoVinculo { get; set; } = string.Empty;

    // Armazena os bytes da foto cadastrada.
    public byte[]? FotoConteudo { get; set; }
}

public class AcessoHistoricoItem
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
}
