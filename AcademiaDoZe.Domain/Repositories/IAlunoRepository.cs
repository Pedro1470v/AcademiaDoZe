
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailJaExiste(
        Email email,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<Aluno?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default);

    Task<Aluno?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Aluno>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default);

    Task<bool> TrocarSenha(
        int id,
        Senha novaSenha,
        CancellationToken cancellationToken = default);
}
