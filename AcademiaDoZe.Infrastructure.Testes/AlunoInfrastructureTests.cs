// Pedro Vinicius

using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    private readonly AlunoRepository _repository;
    private readonly LogradouroRepository _logradouroRepository;

    public AlunoInfrastructureTests()
    {
        _repository = new AlunoRepository(
            ConnectionString);

        _logradouroRepository = new LogradouroRepository(
            ConnectionString);
    }

    private async Task<Aluno> CriarAlunoAsync()
    {
        var logradouroResult = Logradouro.Criar(
            GerarCep(),
            "Pedro Vinicius",
            "Oliveira",
            "SQLite",
            "SC");

        Assert.True(logradouroResult.IsSuccess);

        var logradouro =
            await _logradouroRepository.Adicionar(
                logradouroResult.Value!);

        var fotoResult = Arquivo.Criar(
            new byte[] { 1, 2, 3, 4 });

        Assert.True(fotoResult.IsSuccess);

        var alunoResult = Aluno.Criar(
            0,
            "Pedro Vinicius",
            GerarCpf(),
            new DateOnly(2000, 1, 1),
            GerarTelefone(),
            GerarEmail(),
            logradouro,
            "100",
            "Oliveira",
            "SenhaSQLite123",
            fotoResult.Value!);

        Assert.True(alunoResult.IsSuccess);

        return alunoResult.Value!;
    }

    [Fact]
    public async Task Aluno_Adicionar_Sucesso()
    {
        var aluno = await CriarAlunoAsync();

        var resultado =
            await _repository.Adicionar(aluno);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Aluno_ObterPorId_Sucesso()
    {
        var aluno = await CriarAlunoAsync();

        var alunoAdicionado =
            await _repository.Adicionar(aluno);

        var resultado =
            await _repository.ObterPorId(
                alunoAdicionado.ID);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Aluno_ObterTodos_Sucesso()
    {
        var aluno = await CriarAlunoAsync();

        await _repository.Adicionar(aluno);

        var resultado =
            await _repository.ObterTodos();

        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado);
    }

    [Fact]
    public async Task Aluno_Atualizar_Sucesso()
    {
        var aluno = await CriarAlunoAsync();

        var alunoAdicionado =
            await _repository.Adicionar(aluno);

        var resultado =
            await _repository.Atualizar(
                alunoAdicionado);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Aluno_Remover_Sucesso()
    {
        var aluno = await CriarAlunoAsync();

        var alunoAdicionado =
            await _repository.Adicionar(aluno);

        var resultado =
            await _repository.Remover(
                alunoAdicionado.ID);

        Assert.True(resultado);
    }

    [Fact]
    public async Task Aluno_Adicionar_VariasVezes_Sucesso()
    {
        var aluno1 = await CriarAlunoAsync();

        var aluno2 = await CriarAlunoAsync();

        var resultado1 =
            await _repository.Adicionar(aluno1);

        var resultado2 =
            await _repository.Adicionar(aluno2);

        Assert.NotNull(resultado1);
        Assert.NotNull(resultado2);
    }

    [Fact]
    public async Task Aluno_ObterPorId_Inexistente_RetornaNull()
    {
        var resultado =
            await _repository.ObterPorId(999999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Aluno_Remover_Inexistente_RetornaFalse()
    {
        var resultado =
            await _repository.Remover(999999);

        Assert.False(resultado);
    }
}