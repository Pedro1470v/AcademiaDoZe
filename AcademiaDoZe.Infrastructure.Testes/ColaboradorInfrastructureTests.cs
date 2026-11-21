
// Pedro Vinicius

using System;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class ColaboradorInfrastructureTests : TestBase
{
    private readonly ColaboradorRepository _repository;
    private readonly LogradouroRepository _logradouroRepository;

    public ColaboradorInfrastructureTests()
    {
        _repository = new ColaboradorRepository(ConnectionString);

        _logradouroRepository = new LogradouroRepository(
            ConnectionString);
    }

    private async Task<Colaborador> CriarColaboradorAsync()
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

        var cpf = Cpf.Criar(GerarCpf()).Value!;
        var email = Email.Criar(GerarEmail()).Value!;
        var telefone = Telefone.Criar(GerarTelefone()).Value!;

        var enderecoResult = Endereco.Criar(
            logradouro.Cep,
            logradouro.Nome,
            "100",
            "Oliveira",
            logradouro.Bairro,
            logradouro.Cidade,
            logradouro.Estado);

        Assert.True(enderecoResult.IsSuccess);

        var senhaResult = Senha.Criar("Senha123");

        Assert.True(senhaResult.IsSuccess);

        var colaboradorResult = Colaborador.Criar(
            0,
            "Pedro Vinicius",
            cpf,
            email,
            telefone,
            enderecoResult.Value!,
            new DateOnly(2000, 1, 1),
            new DateOnly(2024, 1, 1),
            senhaResult.Value!,
            null,
            EColaboradorTipo.Instrutor,
            EColaboradorVinculo.CLT);

        Assert.True(colaboradorResult.IsSuccess);

        return colaboradorResult.Value!;
    }

    [Fact]
    public async Task Colaborador_Adicionar_Sucesso()
    {
        var colaborador = await CriarColaboradorAsync();

        var resultado =
            await _repository.Adicionar(colaborador);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Colaborador_ObterPorId_Sucesso()
    {
        var colaborador = await CriarColaboradorAsync();

        await _repository.Adicionar(colaborador);

        var resultado =
            await _repository.ObterPorId(colaborador.ID);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Colaborador_ObterTodos_Sucesso()
    {
        var colaborador = await CriarColaboradorAsync();

        await _repository.Adicionar(colaborador);

        var resultado =
            await _repository.ObterTodos();

        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado);
    }

    [Fact]
    public async Task Colaborador_Atualizar_Sucesso()
    {
        var colaborador = await CriarColaboradorAsync();

        await _repository.Adicionar(colaborador);

        var resultado =
            await _repository.Atualizar(colaborador);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Colaborador_Remover_Sucesso()
    {
        var colaborador = await CriarColaboradorAsync();

        await _repository.Adicionar(colaborador);

        var resultado =
            await _repository.Remover(colaborador.ID);

        Assert.True(resultado);
    }

    [Fact]
    public async Task Colaborador_Adicionar_VariasVezes_Sucesso()
    {
        var colaborador1 = await CriarColaboradorAsync();
        var colaborador2 = await CriarColaboradorAsync();

        var resultado1 =
            await _repository.Adicionar(colaborador1);

        var resultado2 =
            await _repository.Adicionar(colaborador2);

        Assert.NotNull(resultado1);
        Assert.NotNull(resultado2);
    }

    [Fact]
    public async Task Colaborador_ObterPorId_Inexistente_RetornaNull()
    {
        var resultado =
            await _repository.ObterPorId(999999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Colaborador_Remover_Inexistente_RetornaFalse()
    {
        var resultado =
            await _repository.Remover(999999);

        Assert.False(resultado);
    }
}
