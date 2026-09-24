// Pedro Vinicius

using System;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class LogradouroInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _repository;

    public LogradouroInfrastructureTests()
    {
        _repository = new LogradouroRepository(
            ConnectionString);
    }

    internal static async Task<Logradouro> CriarEInserirLogradouroAsync(
        LogradouroRepository logradouroRepo)
    {
        var cep = GerarCep();

        var resultado = Logradouro.Criar(
            cep,
            "Pedro Vinicius",
            "Oliveira",
            "SQLServer",
            "SC");

        if (resultado.IsFailure)
        {
            throw new Exception(
                $"Falha ao criar Logradouro: " +
                $"{string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        n => n.Mensagem))}");
        }

        return await logradouroRepo.Adicionar(
            resultado.Value!);
    }

    [Fact]
    public async Task Logradouro_Adicionar_E_ObterPorId_Sucesso()
    {
        var cep = GerarCep();

        var resultado = Logradouro.Criar(
            cep,
            "Pedro Vinicius",
            "Oliveira",
            "SQLServer",
            "SC");

        Assert.True(
            resultado.IsSuccess,
            string.Join(
                ", ",
                resultado.Notifications.Select(
                    n => n.Mensagem)));

        var inserido = await _repository.Adicionar(
            resultado.Value!);

        Assert.NotNull(inserido);

        Assert.True(
            inserido.ID > 0);

        Assert.Equal(
            cep,
            inserido.Cep);

        Assert.Equal(
            "Pedro Vinicius",
            inserido.Nome);

        Assert.Equal(
            "Oliveira",
            inserido.Bairro);

        Assert.Equal(
            "SQLServer",
            inserido.Cidade);

        Assert.Equal(
            "SC",
            inserido.Estado);

        var obtido = await _repository.ObterPorId(
            inserido.ID);

        Assert.NotNull(obtido);

        Assert.Equal(
            inserido.ID,
            obtido.ID);

        Assert.Equal(
            cep,
            obtido.Cep);

        Assert.Equal(
            "Pedro Vinicius",
            obtido.Nome);

        Assert.Equal(
            "Oliveira",
            obtido.Bairro);

        Assert.Equal(
            "SQLServer",
            obtido.Cidade);

        Assert.Equal(
            "SC",
            obtido.Estado);
    }

    [Fact]
    public async Task Logradouro_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido = await _repository.ObterPorId(
            999999);

        Assert.Null(obtido);
    }

    [Fact]
    public async Task Logradouro_ObterTodos_Sucesso()
    {
        await CriarEInserirLogradouroAsync(
            _repository);

        var todos = await _repository.ObterTodos();

        Assert.NotNull(todos);

        Assert.NotEmpty(todos);

        Assert.Contains(
            todos,
            x =>
                x.Nome == "Pedro Vinicius" &&
                x.Bairro == "Oliveira" &&
                x.Cidade == "SQLServer");
    }

    [Fact]
    public async Task Logradouro_Atualizar_Sucesso()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync(
                _repository);

        var novoCep = GerarCep();

        var resultadoCriacao =
            Logradouro.Criar(
                logradouro.ID,
                novoCep,
                "Pedro Vinicius",
                "Oliveira",
                "SQLServer",
                "SC");

        Assert.True(
            resultadoCriacao.IsSuccess,
            string.Join(
                ", ",
                resultadoCriacao.Notifications.Select(
                    n => n.Mensagem)));

        var resultado =
            await _repository.Atualizar(
                resultadoCriacao.Value!);

        Assert.NotNull(resultado);

        Assert.Equal(
            "Pedro Vinicius",
            resultado.Nome);

        Assert.Equal(
            "Oliveira",
            resultado.Bairro);

        Assert.Equal(
            "SQLServer",
            resultado.Cidade);

        Assert.Equal(
            "SC",
            resultado.Estado);

        Assert.Equal(
            novoCep,
            resultado.Cep);

        var noBanco =
            await _repository.ObterPorId(
                logradouro.ID);

        Assert.NotNull(noBanco);

        Assert.Equal(
            "Pedro Vinicius",
            noBanco.Nome);

        Assert.Equal(
            "Oliveira",
            noBanco.Bairro);

        Assert.Equal(
            "SQLServer",
            noBanco.Cidade);

        Assert.Equal(
            "SC",
            noBanco.Estado);

        Assert.Equal(
            novoCep,
            noBanco.Cep);
    }
}