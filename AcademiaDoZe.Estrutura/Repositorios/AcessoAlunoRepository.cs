
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            return null;

        return await MapearAcessoAsync(reader, cancellationToken);
    }

    public async Task<AcessoAluno?> ObterAcessoEmAbertoPorAluno(
        int alunoId,
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
            WHERE aluno_id = @aluno_id
              AND data_hora_saida IS NULL
            ORDER BY data_hora_entrada DESC
            LIMIT 1;
        ";

        AdicionarParametro(command, "@aluno_id", alunoId);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return await MapearAcessoAsync(reader, cancellationToken);
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
            ORDER BY data_hora_entrada DESC;
        ";

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var acesso = await MapearAcessoAsync(
                reader,
                cancellationToken);

            if (acesso != null)
                lista.Add(acesso);
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

            SELECT last_insert_rowid();
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

        var resultadoId =
            await command.ExecuteScalarAsync(cancellationToken);

        int novoId = Convert.ToInt32(resultadoId);

        var resultado = AcessoAluno.Criar(
            novoId,
            entity.Aluno,
            entity.DataHoraEntrada,
            entity.DataHoraSaida);

        if (resultado.IsFailure || resultado.Value == null)
        {
            throw new InvalidOperationException(
                "Não foi possível reconstruir o acesso com o ID gerado.");
        }

        return resultado.Value;
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

        AdicionarParametro(command, "@id", entity.ID);
        AdicionarParametro(command, "@aluno_id", entity.Aluno.ID);

        AdicionarParametro(
            command,
            "@data_hora_entrada",
            entity.DataHoraEntrada.ToString("O"));

        AdicionarParametro(
            command,
            "@data_hora_saida",
            entity.DataHoraSaida?.ToString("O"));

        int linhas = await command.ExecuteNonQueryAsync(
            cancellationToken);

        if (linhas == 0)
        {
            throw new InvalidOperationException(
                $"Nenhum acesso encontrado com ID {entity.ID}.");
        }

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

        AdicionarParametro(command, "@id", id);

        int resultado = await command.ExecuteNonQueryAsync(
            cancellationToken);

        return resultado > 0;
    }

    private async Task<AcessoAluno?> MapearAcessoAsync(
        System.Data.Common.DbDataReader reader,
        CancellationToken cancellationToken)
    {
        int acessoId = Convert.ToInt32(
            reader["id_acesso_aluno"]);

        int alunoId = Convert.ToInt32(
            reader["aluno_id"]);

        DateTime entrada = DateTime.Parse(
            reader["data_hora_entrada"].ToString()!);

        DateTime? saida = null;

        if (reader["data_hora_saida"] != DBNull.Value)
        {
            saida = DateTime.Parse(
                reader["data_hora_saida"].ToString()!);
        }

        var alunoRepository =
            new AlunoRepository(ConnectionString);

        var aluno = await alunoRepository.ObterPorId(
            alunoId,
            cancellationToken);

        if (aluno == null)
            return null;

        var resultado = AcessoAluno.Criar(
            acessoId,
            aluno,
            entrada,
            saida);

        return resultado.IsSuccess
            ? resultado.Value
            : null;
    }
}
