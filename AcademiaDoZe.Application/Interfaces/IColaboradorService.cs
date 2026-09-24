// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AcademiaDoZe.Application.Interfaces;

public interface IColaboradorService
{
    Task<ColaboradorDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default);

    Task<ColaboradorDto> AdicionarAsync(
        ColaboradorDto colaborador,
        CancellationToken cancellationToken = default);

    Task<ColaboradorDto> AtualizarAsync(
        ColaboradorDto colaborador,
        CancellationToken cancellationToken = default);

    Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ColaboradorDto?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default);

    Task<ColaboradorDto?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ColaboradorDto>> ObterPorNomeAsync(
        string nome,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(
        Domain.Enums.EColaboradorTipo tipo,
        CancellationToken cancellationToken = default);

    Task<bool> CpfJaExisteAsync(
        string cpf,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailJaExisteAsync(
        string email,
        int? id = null,
        CancellationToken cancellationToken = default);
}