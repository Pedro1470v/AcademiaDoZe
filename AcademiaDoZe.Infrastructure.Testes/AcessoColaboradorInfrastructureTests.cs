
// Pedro Vinicius

using System;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AcessoColaboradorInfrastructureTests : TestBase
{
    private readonly AcessoColaboradorRepository _repository;
    private readonly ColaboradorRepository _colaboradorRepository;
    private readonly LogradouroRepository _logradouroRepository;

    public AcessoColaboradorInfrastructureTests()
    {
        _repository =
            new AcessoColaboradorRepository(ConnectionString);

        _colaboradorRepository =
            new ColaboradorRepository(ConnectionString);

        _logradouroRepository =
            new LogradouroRepository(ConnectionString);
    }

    private async Task<Colaborador> CriarColaboradorAsync(
        EColaboradorVinculo vinculo)
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
            vinculo);

        Assert.True(colaboradorResult.IsSuccess);

        return await _colaboradorRepository.Adicionar(
            colaboradorResult.Value!);
    }

    [Fact]
    public async Task Colaborador_CLT_PodeRegistrarEntrada_AntesDoLimite()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var data = DateTime.Today;

        var acesso = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(8),
            data.AddHours(12));

        Assert.True(acesso.IsSuccess);

        await _repository.Adicionar(acesso.Value!);

        var podeRegistrar =
            await _repository.PodeRegistrarEntrada(
                colaborador,
                data.AddHours(13));

        Assert.True(podeRegistrar);
    }

    [Fact]
    public async Task Colaborador_CLT_NaoPodeUltrapassarOitoHoras()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var data = DateTime.Today;

        var acesso = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(8),
            data.AddHours(16));

        Assert.True(acesso.IsSuccess);

        await _repository.Adicionar(acesso.Value!);

        var podeRegistrar =
            await _repository.PodeRegistrarEntrada(
                colaborador,
                data.AddHours(17));

        Assert.False(podeRegistrar);
    }

    [Fact]
    public async Task Colaborador_Estagio_PodeRegistrarEntrada_AntesDoLimite()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.Estagio);

        var data = DateTime.Today;

        var acesso = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(8),
            data.AddHours(11));

        Assert.True(acesso.IsSuccess);

        await _repository.Adicionar(acesso.Value!);

        var podeRegistrar =
            await _repository.PodeRegistrarEntrada(
                colaborador,
                data.AddHours(12));

        Assert.True(podeRegistrar);
    }

    [Fact]
    public async Task Colaborador_Estagio_NaoPodeUltrapassarSeisHoras()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.Estagio);

        var data = DateTime.Today;

        var acesso = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(8),
            data.AddHours(14));

        Assert.True(acesso.IsSuccess);

        await _repository.Adicionar(acesso.Value!);

        var podeRegistrar =
            await _repository.PodeRegistrarEntrada(
                colaborador,
                data.AddHours(15));

        Assert.False(podeRegistrar);
    }

    [Fact]
    public async Task Colaborador_CLT_SomaTodosOsRegistrosDoDia()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var data = DateTime.Today;

        var acesso1 = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(8),
            data.AddHours(10));

        Assert.True(acesso1.IsSuccess);

        await _repository.Adicionar(acesso1.Value!);

        var acesso2 = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(11),
            data.AddHours(14));

        Assert.True(acesso2.IsSuccess);

        await _repository.Adicionar(acesso2.Value!);

        var acesso3 = AcessoColaborador.Criar(
            0,
            colaborador,
            data.AddHours(15),
            data.AddHours(17));

        Assert.True(acesso3.IsSuccess);

        await _repository.Adicionar(acesso3.Value!);

        var tempoTotal =
            await _repository.ObterTempoTotalDoDia(
                colaborador.ID,
                data);

        Assert.Equal(TimeSpan.FromHours(7), tempoTotal);

        var podeRegistrar =
            await _repository.PodeRegistrarEntrada(
                colaborador,
                data.AddHours(18));

        Assert.True(podeRegistrar);
    }

    [Fact]
    public async Task AcessoColaborador_CalcularTempoPermanencia_Sucesso()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var entrada = DateTime.Today.AddHours(8);
        var saida = DateTime.Today.AddHours(12);

        var resultado = AcessoColaborador.Criar(
            0,
            colaborador,
            entrada,
            saida);

        Assert.True(resultado.IsSuccess);

        var tempo =
            resultado.Value!.CalcularTempoPermanencia();

        Assert.Equal(TimeSpan.FromHours(4), tempo);
    }

    [Fact]
    public async Task AcessoColaborador_RegistrarSaida_Sucesso()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var entrada = DateTime.Today.AddHours(8);

        var resultado = AcessoColaborador.Criar(
            0,
            colaborador,
            entrada);

        Assert.True(resultado.IsSuccess);

        var acesso = resultado.Value!;
        var saida = DateTime.Today.AddHours(12);

        var resultadoSaida =
            acesso.RegistrarSaida(saida);

        Assert.True(resultadoSaida.IsSuccess);

        Assert.Equal(saida, acesso.DataHoraSaida);

        Assert.Equal(
            TimeSpan.FromHours(4),
            acesso.TempoPermanencia);
    }

    [Fact]
    public async Task AcessoColaborador_RegistrarSaida_MenorQueEntrada_Falha()
    {
        var colaborador =
            await CriarColaboradorAsync(EColaboradorVinculo.CLT);

        var entrada = DateTime.Today.AddHours(12);

        var resultado = AcessoColaborador.Criar(
            0,
            colaborador,
            entrada);

        Assert.True(resultado.IsSuccess);

        var acesso = resultado.Value!;

        var resultadoSaida =
            acesso.RegistrarSaida(
                DateTime.Today.AddHours(10));

        Assert.True(resultadoSaida.IsFailure);

        Assert.Contains(
            resultadoSaida.Notifications,
            x => x.Property == "DataHoraSaida");
    }
}
