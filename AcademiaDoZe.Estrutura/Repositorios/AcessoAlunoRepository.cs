// AcessoAlunoRepository.cs

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Data;

public class AcessoAlunoRepository : BaseRepository, IAcessoAlunoRepository
{
    public AcessoAlunoRepository(string connectionString)
        : base(connectionString, DatabaseType.Sqlite)
    {
    }

    public async Task<AcessoAluno?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                id_acesso_aluno,
                aluno_id,
                data_hora_entrada,
                data_hora_saida
            FROM tb_acesso_aluno
            WHERE id_acesso_aluno = @id;
        ";

        AdicionarParametro(command, "@id", id);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        int acessoId = reader.GetInt32(
            reader.GetOrdinal("id_acesso_aluno"));

        int alunoId = reader.GetInt32(
            reader.GetOrdinal("aluno_id"));

        DateTime entrada = DateTime.Parse(
            reader.GetString(
                reader.GetOrdinal("data_hora_entrada")));

        DateTime? saida = null;

        int ordinalSaida =
            reader.GetOrdinal("data_hora_saida");

        if (!reader.IsDBNull(ordinalSaida))
        {
            saida = DateTime.Parse(
                reader.GetString(ordinalSaida));
        }

        var alunoRepository =
            new AlunoRepository(ConnectionString);

        var aluno = await alunoRepository.ObterPorId(
            alunoId,
            cancellationToken);

        if (aluno == null)
        {
            return null;
        }

        var resultado = AcessoAluno.Criar(
            acessoId,
            aluno,
            entrada,
            saida);

        if (resultado.IsFailure)
        {
            return null;
        }

        return resultado.Value;
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var lista = new List<AcessoAluno>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                id_acesso_aluno,
                aluno_id,
                data_hora_entrada,
                data_hora_saida
            FROM tb_acesso_aluno
            ORDER BY id_acesso_aluno;
        ";

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        var alunoRepository =
            new AlunoRepository(ConnectionString);

        while (await reader.ReadAsync(cancellationToken))
        {
            int acessoId = reader.GetInt32(
                reader.GetOrdinal("id_acesso_aluno"));

            int alunoId = reader.GetInt32(
                reader.GetOrdinal("aluno_id"));

            DateTime entrada = DateTime.Parse(
                reader.GetString(
                    reader.GetOrdinal("data_hora_entrada")));

            DateTime? saida = null;

            int ordinalSaida =
                reader.GetOrdinal("data_hora_saida");

            if (!reader.IsDBNull(ordinalSaida))
            {
                saida = DateTime.Parse(
                    reader.GetString(ordinalSaida));
            }

            var aluno = await alunoRepository.ObterPorId(
                alunoId,
                cancellationToken);

            if (aluno == null)
            {
                continue;
            }

            var resultado = AcessoAluno.Criar(
                acessoId,
                aluno,
                entrada,
                saida);

            if (resultado.IsSuccess &&
                resultado.Value != null)
            {
                lista.Add(resultado.Value);
            }
        }

        return lista;
    }

    public async Task<AcessoAluno> Adicionar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO tb_acesso_aluno
            (
                aluno_id,
                data_hora_entrada,
                data_hora_saida
            )
            VALUES
            (
                @aluno_id,
                @data_hora_entrada,
                @data_hora_saida
            );
        ";

        AdicionarParametro(
            command,
            "@aluno_id",
            entity.Aluno.ID);

        AdicionarParametro(
            command,
            "@data_hora_entrada",
            entity.DataHoraEntrada.ToString("O"));

        AdicionarParametro(
            command,
            "@data_hora_saida",
            entity.DataHoraSaida?.ToString("O"));

        await command.ExecuteNonQueryAsync(
            cancellationToken);

        return entity;
    }

    public async Task<AcessoAluno> Atualizar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE tb_acesso_aluno
            SET
                aluno_id = @aluno_id,
                data_hora_entrada = @data_hora_entrada,
                data_hora_saida = @data_hora_saida
            WHERE id_acesso_aluno = @id;
        ";

        AdicionarParametro(
            command,
            "@id",
            entity.ID);

        AdicionarParametro(
            command,
            "@aluno_id",
            entity.Aluno.ID);

        AdicionarParametro(
            command,
            "@data_hora_entrada",
            entity.DataHoraEntrada.ToString("O"));

        AdicionarParametro(
            command,
            "@data_hora_saida",
            entity.DataHoraSaida?.ToString("O"));

        await command.ExecuteNonQueryAsync(
            cancellationToken);

        return entity;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM tb_acesso_aluno
            WHERE id_acesso_aluno = @id;
        ";

        AdicionarParametro(
            command,
            "@id",
            id);

        int resultado =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        return resultado > 0;
    }
}