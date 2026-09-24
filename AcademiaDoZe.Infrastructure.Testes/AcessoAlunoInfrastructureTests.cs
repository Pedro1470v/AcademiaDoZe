// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AcessoAlunoInfrastructureTests : TestBase
{
    private readonly AcessoAlunoRepository _repository;
    private readonly AlunoRepository _alunoRepository;
    private readonly MatriculaRepository _matriculaRepository;
    private readonly LogradouroRepository _logradouroRepository;

    public AcessoAlunoInfrastructureTests()
    {
        _repository =
            new AcessoAlunoRepository(
                ConnectionString);

        _alunoRepository =
            new AlunoRepository(
                ConnectionString);

        _matriculaRepository =
            new MatriculaRepository(
                ConnectionString);

        _logradouroRepository =
            new LogradouroRepository(
                ConnectionString);
    }

    [Fact]
    public async Task DeveRegistrarEntradaDeAlunoComMatriculaAtiva()
    {
        var logradouroResult =
            Logradouro.Criar(
                "88000000",
                "Pedro Vinicius",
                "Oliveira",
                "SQLite",
                "SC");

        Assert.True(
            logradouroResult.IsSuccess);

        var logradouro =
            logradouroResult.Value!;

        logradouro =
            await _logradouroRepository.Adicionar(
                logradouro);

        var arquivoResult =
            Arquivo.Criar(
                new byte[] { 1, 2, 3, 4 });

        Assert.True(
            arquivoResult.IsSuccess);

        var alunoResult =
            Aluno.Criar(
                0,
                "Aluno Teste Acesso",
                "12345678909",
                DateOnly.FromDateTime(
                    DateTime.Today.AddYears(-20)),
                "49999999999",
                "alunoacesso@teste.com",
                logradouro,
                "100",
                "",
                "Senha123",
                arquivoResult.Value!);

        Assert.True(
            alunoResult.IsSuccess);

        var aluno =
            alunoResult.Value!;

        aluno =
            await _alunoRepository.Adicionar(
                aluno);

        var matriculaResult =
            Matricula.Criar(
                0,
                aluno,
                MatriculaPlano.Mensal,
                DateOnly.FromDateTime(
                    DateTime.Today),
                "Condicionamento físico",
                (MatriculaRestricoes)0,
                null,
                string.Empty);

        Assert.True(
            matriculaResult.IsSuccess);

        var matricula =
            matriculaResult.Value!;

        await _matriculaRepository.Adicionar(
            matricula);

        var acessoResult =
            AcessoAluno.Criar(
                0,
                aluno,
                DateTime.Now);

        Assert.True(
            acessoResult.IsSuccess);

        var acesso =
            acessoResult.Value!;

        var resultado =
            await _repository.Adicionar(
                acesso);

        Assert.NotNull(
            resultado);

        Assert.Equal(
            aluno.ID,
            resultado.Aluno.ID);

        Assert.Null(
            resultado.DataHoraSaida);
    }
}