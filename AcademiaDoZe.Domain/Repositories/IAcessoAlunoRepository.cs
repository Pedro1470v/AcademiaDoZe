
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
{
    Task<AcessoAluno?> ObterAcessoEmAbertoPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default);
}
