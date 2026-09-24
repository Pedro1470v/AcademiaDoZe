// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Repositories;

public interface IColaboradorRepository : IRepository<Colaborador>
{
    Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailJaExiste(
        Email email,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<Colaborador?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default);

    Task<Colaborador?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Colaborador>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default);

    Task<bool> TrocarSenha(
        int id,
        Senha novaSenha,
        CancellationToken cancellationToken = default);
}