
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoColaboradorService
{
    Task<AcessoColaboradorDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AcessoColaboradorDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default);

    Task<AcessoColaboradorDto> RegistrarEntradaAsync(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<AcessoColaboradorDto> RegistrarSaidaAsync(
        int acessoId,
        CancellationToken cancellationToken = default);

    Task<TimeSpan> ObterTempoTotalDoDiaAsync(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default);

    Task<bool> PodeRegistrarEntradaAsync(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default);
}
