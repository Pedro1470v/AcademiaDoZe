// Pedro Vinicius

using System;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoColaboradorRepository
        : IRepository<AcessoColaborador>
    {
        Task<TimeSpan> ObterTempoTotalDoDia(
            int colaboradorId,
            DateTime data,
            CancellationToken cancellationToken = default);

        Task<bool> PodeRegistrarEntrada(
            Colaborador colaborador,
            DateTime dataHoraEntrada,
            CancellationToken cancellationToken = default);
    }
}