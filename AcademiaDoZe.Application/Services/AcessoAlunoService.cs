//Pedro Vinicius
using System;
using System.Collections.Generic;
using System.Linq;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoAlunoService
{
    private readonly Func<IAcessoAlunoRepository> _acessoAlunoRepoFactory;
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;

    public AcessoAlunoService(
        Func<IAcessoAlunoRepository> acessoAlunoRepoFactory,
        Func<IMatriculaRepository> matriculaRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory)
    {
        _acessoAlunoRepoFactory =
            acessoAlunoRepoFactory
            ?? throw new ArgumentNullException(
                nameof(acessoAlunoRepoFactory));

        _matriculaRepoFactory =
            matriculaRepoFactory
            ?? throw new ArgumentNullException(
                nameof(matriculaRepoFactory));

        _alunoRepoFactory =
            alunoRepoFactory
            ?? throw new ArgumentNullException(
                nameof(alunoRepoFactory));
    }

    public async Task<AcessoAluno?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _acessoAlunoRepoFactory()
            .ObterPorId(
                id,
                cancellationToken);
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _acessoAlunoRepoFactory()
            .ObterTodos(
                cancellationToken);
    }

    public async Task<AcessoAluno> RegistrarEntradaAsync(
        int alunoId,
        DateTime dataHoraEntrada,
        CancellationToken cancellationToken = default)
    {
        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(
                    alunoId,
                    cancellationToken);

        if (aluno == null)
        {
            throw new KeyNotFoundException(
                $"Aluno com ID {alunoId} não encontrado.");
        }

        var possuiMatriculaAtiva =
            await _matriculaRepoFactory()
                .PossuiMatriculaAtiva(
                    alunoId,
                    cancellationToken);

        if (!possuiMatriculaAtiva)
        {
            throw new InvalidOperationException(
                "O aluno não possui matrícula ativa e não pode registrar entrada.");
        }

        var resultado =
            AcessoAluno.Criar(
                0,
                aluno,
                dataHoraEntrada);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        return await _acessoAlunoRepoFactory()
            .Adicionar(
                resultado.Value!,
                cancellationToken);
    }

    public async Task<AcessoAluno> RegistrarSaidaAsync(
        int acessoId,
        DateTime dataHoraSaida,
        CancellationToken cancellationToken = default)
    {
        var acesso =
            await _acessoAlunoRepoFactory()
                .ObterPorId(
                    acessoId,
                    cancellationToken);

        if (acesso == null)
        {
            throw new KeyNotFoundException(
                $"Acesso do aluno com ID {acessoId} não encontrado.");
        }

        var resultado =
            acesso.RegistrarSaida(
                dataHoraSaida);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        return await _acessoAlunoRepoFactory()
            .Atualizar(
                acesso,
                cancellationToken);
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _acessoAlunoRepoFactory()
            .Remover(
                id,
                cancellationToken);
    }
}