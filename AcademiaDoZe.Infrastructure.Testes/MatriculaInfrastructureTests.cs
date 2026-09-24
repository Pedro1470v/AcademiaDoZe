// Pedro Vinicius

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Tests;
using System;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Testes
{
    public class MatriculaInfrastructureTests : TestBase
    {
        private readonly MatriculaRepository _repository;
        private readonly AlunoRepository _alunoRepository;
        private readonly LogradouroRepository _logradouroRepository;

        public MatriculaInfrastructureTests()
        {
            _repository = new MatriculaRepository(
                ConnectionString);

            _alunoRepository = new AlunoRepository(
                ConnectionString);

            _logradouroRepository = new LogradouroRepository(
                ConnectionString);
        }

        [Fact]
        public async Task Matricula_Adicionar_E_ObterPorId_Sucesso()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            var adicionada =
                await _repository.Adicionar(
                    resultado.Value!);

            Assert.NotNull(adicionada);
            Assert.True(adicionada.ID > 0);

            var obtida =
                await _repository.ObterPorId(
                    adicionada.ID);

            Assert.NotNull(obtida);

            Assert.Equal(
                adicionada.ID,
                obtida!.ID);

            Assert.Equal(
                aluno.ID,
                obtida.Aluno.ID);

            Assert.Equal(
                MatriculaPlano.Mensal,
                obtida.Plano);

            Assert.Equal(
                "Pedro Vinicius",
                obtida.Objetivo);

            Assert.Equal(
                "SQLite",
                obtida.ObservacoesRestricoes);
        }

        [Fact]
        public async Task Matricula_Atualizar_LancaExcecaoQuandoInexistente()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            var matricula = resultado.Value!;

            var excecao =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () =>
                    {
                        await _repository.Atualizar(
                            matricula);
                    });

            Assert.NotNull(excecao);
        }

        [Fact]
        public async Task Matricula_Atualizar_Sucesso()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            var adicionada =
                await _repository.Adicionar(
                    resultado.Value!);

            adicionada.AlterarObjetivo(
                "Pedro Vinicius");

            adicionada.AlterarPlano(
                MatriculaPlano.Trimestral);

            var atualizada =
                await _repository.Atualizar(
                    adicionada);

            Assert.NotNull(atualizada);

            var obtida =
                await _repository.ObterPorId(
                    adicionada.ID);

            Assert.NotNull(obtida);

            Assert.Equal(
                "Pedro Vinicius",
                obtida!.Objetivo);

            Assert.Equal(
                "SQLite",
                obtida.ObservacoesRestricoes);

            Assert.Equal(
                MatriculaPlano.Trimestral,
                obtida.Plano);
        }

        [Fact]
        public async Task Matricula_ObterAtivas_FiltragemCorreta()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            await _repository.Adicionar(
                resultado.Value!);

            var matriculas =
                await _repository.ObterAtivas();

            Assert.NotEmpty(matriculas);

            Assert.Contains(
                matriculas,
                x => x.Aluno.ID == aluno.ID);

            Assert.All(
                matriculas,
                x => Assert.True(
                    x.DataFim >=
                    DateOnly.FromDateTime(
                        DateTime.Today)));
        }

        [Fact]
        public async Task Matricula_ObterMatriculaAtivaPorAluno_E_PossuiMatriculaAtiva()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Anual,
                MatriculaRestricoes.Alergias);

            Assert.True(resultado.IsSuccess);

            await _repository.Adicionar(
                resultado.Value!);

            var matricula =
                await _repository.ObterMatriculaAtivaPorAluno(
                    aluno.ID);

            Assert.NotNull(matricula);

            Assert.Equal(
                aluno.ID,
                matricula!.Aluno.ID);

            var possuiMatricula =
                await _repository.PossuiMatriculaAtiva(
                    aluno.ID);

            Assert.True(possuiMatricula);
        }

        [Fact]
        public async Task Matricula_ObterPorAluno_FiltragemCorreta()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.PressaoAlta);

            Assert.True(resultado.IsSuccess);

            await _repository.Adicionar(
                resultado.Value!);

            var matriculas =
                await _repository.ObterPorAluno(
                    aluno.ID);

            Assert.NotEmpty(matriculas);

            Assert.All(
                matriculas,
                x => Assert.Equal(
                    aluno.ID,
                    x.Aluno.ID));

            Assert.Contains(
                matriculas,
                x => x.Objetivo == "Pedro Vinicius");
        }

        [Fact]
        public async Task Matricula_ObterPorId_RetornaNuloQuandoInexistente()
        {
            var matricula =
                await _repository.ObterPorId(
                    999999);

            Assert.Null(matricula);
        }

        [Fact]
        public async Task Matricula_ObterPorPlano_FiltragemCorreta()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Anual,
                MatriculaRestricoes.Alergias);

            Assert.True(resultado.IsSuccess);

            await _repository.Adicionar(
                resultado.Value!);

            var matriculas =
                await _repository.ObterPorPlano(
                    MatriculaPlano.Anual);

            Assert.NotEmpty(matriculas);

            Assert.Contains(
                matriculas,
                x =>
                    x.Plano == MatriculaPlano.Anual &&
                    x.Objetivo == "Pedro Vinicius");
        }

        [Fact]
        public async Task Matricula_ObterTodos_Sucesso()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            await _repository.Adicionar(
                resultado.Value!);

            var matriculas =
                await _repository.ObterTodos();

            Assert.NotNull(matriculas);
            Assert.NotEmpty(matriculas);

            Assert.Contains(
                matriculas,
                x => x.Aluno.ID == aluno.ID);
        }

        [Fact]
        public async Task Matricula_ObterVencendoEmDias_RetornaMatriculasProximasDoVencimento()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Labirintite);

            Assert.True(resultado.IsSuccess);

            var adicionada =
                await _repository.Adicionar(
                    resultado.Value!);

            var matriculas =
                await _repository.ObterVencendoEmDias(
                    31);

            Assert.NotEmpty(matriculas);

            Assert.Contains(
                matriculas,
                x => x.ID == adicionada.ID);

            Assert.Contains(
                matriculas,
                x => x.Objetivo == "Pedro Vinicius");
        }

        [Fact]
        public async Task Matricula_Remover_RetornaFalseQuandoInexistente()
        {
            var removido =
                await _repository.Remover(
                    999999);

            Assert.False(removido);
        }

        [Fact]
        public async Task Matricula_Remover_Sucesso()
        {
            var aluno = await CriarAlunoAsync();

            var resultado = CriarMatricula(
                aluno,
                MatriculaPlano.Mensal,
                MatriculaRestricoes.Diabetes);

            Assert.True(resultado.IsSuccess);

            var adicionada =
                await _repository.Adicionar(
                    resultado.Value!);

            var removido =
                await _repository.Remover(
                    adicionada.ID);

            Assert.True(removido);

            var matricula =
                await _repository.ObterPorId(
                    adicionada.ID);

            Assert.Null(matricula);
        }

        [Fact]
        public async Task Matricula_RestricoesMedicas_ComVariacoesMúltiplasEscolha_PersisteEObtemCorretamente()
        {
            var aluno = await CriarAlunoAsync();

            var restricoes =
                MatriculaRestricoes.Diabetes
                | MatriculaRestricoes.Alergias
                | MatriculaRestricoes.PressaoAlta;

            var resultado =
                Matricula.Criar(
                    0,
                    aluno,
                    MatriculaPlano.Semestral,
                    DateOnly.FromDateTime(
                        DateTime.Today),
                    "Pedro Vinicius",
                    restricoes,
                    null,
                    "SQLite");

            Assert.True(resultado.IsSuccess);

            var adicionada =
                await _repository.Adicionar(
                    resultado.Value!);

            var obtida =
                await _repository.ObterPorId(
                    adicionada.ID);

            Assert.NotNull(obtida);

            Assert.Equal(
                restricoes,
                obtida!.RestricoesMedicas);

            Assert.Equal(
                "Pedro Vinicius",
                obtida.Objetivo);

            Assert.Equal(
                "SQLite",
                obtida.ObservacoesRestricoes);
        }

        private static Result<Matricula> CriarMatricula(
            Aluno aluno,
            MatriculaPlano plano,
            MatriculaRestricoes restricoes)
        {
            return Matricula.Criar(
                0,
                aluno,
                plano,
                DateOnly.FromDateTime(
                    DateTime.Today),
                "Pedro Vinicius",
                restricoes,
                null,
                "SQLite");
        }

        private async Task<Aluno> CriarAlunoAsync()
        {
            var logradouroResultado =
                Logradouro.Criar(
                    0,
                    GerarCep(),
                    "Pedro Vinicius",
                    "Oliveira",
                    "SQLite",
                    "SC");

            Assert.True(
                logradouroResultado.IsSuccess);

            var logradouro =
                logradouroResultado.Value!;

            var logradouroSalvo =
                await _logradouroRepository.Adicionar(
                    logradouro);

            Assert.NotNull(
                logradouroSalvo);

            var arquivoResultado =
                Arquivo.Criar(
                    new byte[]
                    {
                        1,
                        2,
                        3,
                        4
                    });

            Assert.True(
                arquivoResultado.IsSuccess);

            var alunoResultado =
                Aluno.Criar(
                    0,
                    "Aluno Teste",
                    "12345678901",
                    DateOnly.FromDateTime(
                        DateTime.Today.AddYears(-20)),
                    "49999999999",
                    "aluno@teste.com",
                    logradouroSalvo,
                    "10",
                    null,
                    "123456",
                    arquivoResultado.Value);

            Assert.True(
                alunoResultado.IsSuccess);

            var aluno =
                alunoResultado.Value!;

            var alunoId =
                await _alunoRepository.InserirAsync(
                    aluno);

            Assert.True(
                alunoId > 0);

            var alunoSalvo =
                await _alunoRepository.ObterPorId(
                    alunoId);

            Assert.NotNull(
                alunoSalvo);

            return alunoSalvo!;
        }

        private static string GerarCep()
        {
            return DateTime.Now.Ticks
                .ToString()
                .Substring(0, 8);
        }
    }
}