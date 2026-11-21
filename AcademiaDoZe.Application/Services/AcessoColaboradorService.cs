
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoColaboradorService
{
    private readonly Func<IAcessoColaboradorRepository> _acessoRepoFactory;
    private readonly Func<IColaboradorRepository> _colaboradorRepoFactory;

    public AcessoColaboradorService(
        Func<IAcessoColaboradorRepository> acessoRepoFactory,
        Func<IColaboradorRepository> colaboradorRepoFactory)
    {
        _acessoRepoFactory = acessoRepoFactory
            ?? throw new ArgumentNullException(nameof(acessoRepoFactory));

        _colaboradorRepoFactory = colaboradorRepoFactory
            ?? throw new ArgumentNullException(nameof(colaboradorRepoFactory));
    }

    public async Task<AcessoColaborador?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _acessoRepoFactory()
            .ObterPorId(id, cancellationToken);
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _acessoRepoFactory()
            .ObterTodos(cancellationToken);
    }

    public async Task<AcessoColaborador> RegistrarEntradaAsync(
        int colaboradorId,
        DateTime dataHoraEntrada,
        CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradorRepoFactory()
            .ObterPorId(colaboradorId, cancellationToken);

        if (colaborador == null)
        {
            throw new KeyNotFoundException(
                $"Colaborador com ID {colaboradorId} não encontrado.");
        }

        var acessos = await _acessoRepoFactory()
            .ObterTodos(cancellationToken);

        bool possuiAcessoAberto = acessos.Any(a =>
            a.Colaborador.ID == colaboradorId &&
            a.DataHoraSaida == null);

        if (possuiAcessoAberto)
        {
            throw new InvalidOperationException(
                "O colaborador já possui uma entrada em aberto.");
        }

        var tempoTotalDoDia = await _acessoRepoFactory()
            .ObterTempoTotalDoDia(
                colaboradorId,
                dataHoraEntrada.Date,
                cancellationToken);

        TimeSpan limiteDiario =
            colaborador.Vinculo == EColaboradorVinculo.Estagio
                ? TimeSpan.FromHours(6)
                : TimeSpan.FromHours(8);

        if (tempoTotalDoDia >= limiteDiario)
        {
            throw new InvalidOperationException(
                colaborador.Vinculo == EColaboradorVinculo.Estagio
                    ? "O colaborador já atingiu o limite diário de 6 horas do estágio."
                    : "O colaborador já atingiu o limite diário de 8 horas CLT.");
        }

        var resultado = AcessoColaborador.Criar(
            0,
            colaborador,
            dataHoraEntrada);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(x => x.Mensagem)));
        }

        return await _acessoRepoFactory()
            .Adicionar(resultado.Value!, cancellationToken);
    }

    public async Task<AcessoColaborador> RegistrarSaidaAsync(
        int acessoId,
        DateTime dataHoraSaida,
        CancellationToken cancellationToken = default)
    {
        var acesso = await _acessoRepoFactory()
            .ObterPorId(acessoId, cancellationToken);

        if (acesso == null)
        {
            throw new KeyNotFoundException(
                $"Acesso do colaborador com ID {acessoId} não encontrado.");
        }

        if (acesso.DataHoraSaida.HasValue)
        {
            throw new InvalidOperationException(
                "Este acesso já possui uma saída registrada.");
        }

        var tempoTotalDoDia = await _acessoRepoFactory()
            .ObterTempoTotalDoDia(
                acesso.Colaborador.ID,
                acesso.DataHoraEntrada.Date,
                cancellationToken);

        TimeSpan permanenciaAtual =
            dataHoraSaida - acesso.DataHoraEntrada;

        var validacaoLimite = acesso.ValidarLimiteDiario(
            tempoTotalDoDia + permanenciaAtual);

        if (validacaoLimite.IsFailure)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    validacaoLimite.Notifications.Select(x => x.Mensagem)));
        }

        var resultado = acesso.RegistrarSaida(dataHoraSaida);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(x => x.Mensagem)));
        }

        return await _acessoRepoFactory()
            .Atualizar(acesso, cancellationToken);
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _acessoRepoFactory()
            .Remover(id, cancellationToken);
    }
}
