// Pedro Vinicius

using System;
using System.Collections.Generic;
using System.Data.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class MatriculaRepository : BaseRepository, IMatriculaRepository
{
    public MatriculaRepository(string connectionString)
        : base(connectionString, DatabaseType.Sqlite)
    {
    }

    public async Task<Matricula?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE m.id_matricula = @id;
        ";

        AdicionarParametro(command, "@id", id);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return await MapAsync(
            connection,
            reader,
            cancellationToken);
    }

    public async Task<IEnumerable<Matricula>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Matricula>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            ORDER BY m.id_matricula;
        ";

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var matricula =
                await MapAsync(
                    connection,
                    reader,
                    cancellationToken);

            if (matricula != null)
            {
                lista.Add(matricula);
            }
        }

        return lista;
    }

    public async Task<Matricula> Adicionar(
        Matricula matricula,
        CancellationToken cancellationToken = default)
    {
        var id = await InserirAsync(
            matricula,
            cancellationToken);

        var resultado =
            await ObterPorId(
                id,
                cancellationToken);

        if (resultado == null)
        {
            throw new InvalidOperationException(
                "A matrícula foi inserida, mas não foi possível recuperá-la.");
        }

        return resultado;
    }

    public async Task<Matricula> Atualizar(
        Matricula matricula,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE tb_matricula
            SET
                aluno_id = @aluno_id,
                plano = @plano,
                data_inicio = @data_inicio,
                data_fim = @data_fim,
                objetivo = @objetivo,
                restricao_medica = @restricao_medica,
                obs_restricao = @obs_restricao,
                laudo_medico = @laudo_medico
            WHERE id_matricula = @id;
        ";

        AdicionarParametro(
            command,
            "@id",
            matricula.ID);

        AdicionarParametro(
            command,
            "@aluno_id",
            matricula.Aluno.ID);

        AdicionarParametro(
            command,
            "@plano",
            (int)matricula.Plano);

        AdicionarParametro(
            command,
            "@data_inicio",
            matricula.DataInicio.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@data_fim",
            matricula.DataFim.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@objetivo",
            matricula.Objetivo);

        AdicionarParametro(
            command,
            "@restricao_medica",
            (int)matricula.RestricoesMedicas);

        AdicionarParametro(
            command,
            "@obs_restricao",
            matricula.ObservacoesRestricoes);

        AdicionarParametro(
            command,
            "@laudo_medico",
            ObterBytesLaudo(matricula.LaudoMedico));

        var linhas =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        if (linhas == 0)
        {
            throw new InvalidOperationException(
                $"Nenhuma matrícula encontrada com ID {matricula.ID}.");
        }

        var resultado =
            await ObterPorId(
                matricula.ID,
                cancellationToken);

        if (resultado == null)
        {
            throw new InvalidOperationException(
                "A matrícula foi atualizada, mas não foi possível recuperá-la.");
        }

        return resultado;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM tb_matricula
            WHERE id_matricula = @id;
        ";

        AdicionarParametro(
            command,
            "@id",
            id);

        var linhas =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        return linhas > 0;
    }

    public async Task<int> InserirAsync(
        Matricula matricula,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO tb_matricula
            (
                aluno_id,
                plano,
                data_inicio,
                data_fim,
                objetivo,
                restricao_medica,
                obs_restricao,
                laudo_medico
            )
            VALUES
            (
                @aluno_id,
                @plano,
                @data_inicio,
                @data_fim,
                @objetivo,
                @restricao_medica,
                @obs_restricao,
                @laudo_medico
            );

            SELECT last_insert_rowid();
        ";

        AdicionarParametro(
            command,
            "@aluno_id",
            matricula.Aluno.ID);

        AdicionarParametro(
            command,
            "@plano",
            (int)matricula.Plano);

        AdicionarParametro(
            command,
            "@data_inicio",
            matricula.DataInicio.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@data_fim",
            matricula.DataFim.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@objetivo",
            matricula.Objetivo);

        AdicionarParametro(
            command,
            "@restricao_medica",
            (int)matricula.RestricoesMedicas);

        AdicionarParametro(
            command,
            "@obs_restricao",
            matricula.ObservacoesRestricoes);

        AdicionarParametro(
            command,
            "@laudo_medico",
            ObterBytesLaudo(matricula.LaudoMedico));

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado);
    }

    public async Task<IEnumerable<Matricula>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Matricula>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE m.aluno_id = @aluno_id
            ORDER BY m.id_matricula;
        ";

        AdicionarParametro(
            command,
            "@aluno_id",
            alunoId);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var matricula =
                await MapAsync(
                    connection,
                    reader,
                    cancellationToken);

            if (matricula != null)
            {
                lista.Add(matricula);
            }
        }

        return lista;
    }

    public async Task<Matricula?> ObterMatriculaAtivaPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE m.aluno_id = @aluno_id
              AND date(m.data_fim) >= date('now')
            ORDER BY m.data_fim DESC
            LIMIT 1;
        ";

        AdicionarParametro(
            command,
            "@aluno_id",
            alunoId);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return await MapAsync(
            connection,
            reader,
            cancellationToken);
    }

    public async Task<bool> PossuiMatriculaAtiva(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT COUNT(*)
            FROM tb_matricula
            WHERE aluno_id = @aluno_id
              AND date(data_fim) >= date('now');
        ";

        AdicionarParametro(
            command,
            "@aluno_id",
            alunoId);

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado) > 0;
    }

    public async Task<IEnumerable<Matricula>> ObterAtivas(
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Matricula>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE date(m.data_fim) >= date('now')
            ORDER BY m.data_fim;
        ";

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var matricula =
                await MapAsync(
                    connection,
                    reader,
                    cancellationToken);

            if (matricula != null)
            {
                lista.Add(matricula);
            }
        }

        return lista;
    }

    public async Task<IEnumerable<Matricula>> ObterVencendoEmDias(
        int dias,
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Matricula>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE date(m.data_fim)
                  BETWEEN date('now')
                  AND date('now', '+' || @dias || ' days')
            ORDER BY m.data_fim;
        ";

        AdicionarParametro(
            command,
            "@dias",
            dias);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var matricula =
                await MapAsync(
                    connection,
                    reader,
                    cancellationToken);

            if (matricula != null)
            {
                lista.Add(matricula);
            }
        }

        return lista;
    }

    public async Task<IEnumerable<Matricula>> ObterPorPlano(
        MatriculaPlano plano,
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Matricula>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                m.id_matricula,
                m.aluno_id,
                m.plano,
                m.data_inicio,
                m.data_fim,
                m.objetivo,
                m.restricao_medica,
                m.obs_restricao,
                m.laudo_medico
            FROM tb_matricula m
            WHERE m.plano = @plano
            ORDER BY m.id_matricula;
        ";

        AdicionarParametro(
            command,
            "@plano",
            (int)plano);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var matricula =
                await MapAsync(
                    connection,
                    reader,
                    cancellationToken);

            if (matricula != null)
            {
                lista.Add(matricula);
            }
        }

        return lista;
    }

    private static async Task<Matricula?> MapAsync(
        DbConnection connection,
        DbDataReader reader,
        CancellationToken cancellationToken)
    {
        var alunoId =
            Convert.ToInt32(
                reader["aluno_id"]);

        var alunoRepository =
            new AlunoRepository(
                connection.ConnectionString!);

        var aluno =
            await alunoRepository.ObterPorId(
                alunoId,
                cancellationToken);

        if (aluno == null)
        {
            return null;
        }

        var plano =
            (MatriculaPlano)Convert.ToInt32(
                reader["plano"]);

        var dataInicio =
            DateOnly.Parse(
                reader["data_inicio"].ToString()!);

        var objetivo =
            reader["objetivo"].ToString() ?? string.Empty;

        var restricoes =
            (MatriculaRestricoes)Convert.ToInt32(
                reader["restricao_medica"]);

        string? observacoes = null;

        if (reader["obs_restricao"] != DBNull.Value)
        {
            observacoes =
                reader["obs_restricao"].ToString();
        }

        Arquivo? laudo = null;

        if (reader["laudo_medico"] != DBNull.Value)
        {
            var bytes =
                (byte[])reader["laudo_medico"];

            var resultadoArquivo =
                Arquivo.Criar(bytes);

            if (resultadoArquivo.IsSuccess)
            {
                laudo = resultadoArquivo.Value;
            }
        }

        var resultado =
            Matricula.Criar(
                Convert.ToInt32(
                    reader["id_matricula"]),
                aluno,
                plano,
                dataInicio,
                objetivo,
                restricoes,
                laudo,
                observacoes);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível reconstruir a matrícula.");
        }

        return resultado.Value;
    }

    private static byte[]? ObterBytesLaudo(
        Arquivo? arquivo)
    {
        if (arquivo == null)
        {
            return null;
        }

        return arquivo.Conteudo;
    }

    private static void AdicionarParametro(
        DbCommand command,
        string nome,
        object? valor)
    {
        var parametro =
            command.CreateParameter();

        parametro.ParameterName = nome;
        parametro.Value =
            valor ?? DBNull.Value;

        command.Parameters.Add(parametro);
    }
}