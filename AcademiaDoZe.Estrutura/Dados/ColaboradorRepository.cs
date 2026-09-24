
// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class ColaboradorRepository : BaseRepository, IColaboradorRepository
{
    public ColaboradorRepository(string connectionString)
        : base(connectionString, DatabaseType.Sqlite)
    {
    }

    public async Task<Colaborador?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                c.id_colaborador,
                c.nome,
                c.cpf,
                c.email,
                c.telefone,
                c.logradouro_id,
                c.numero,
                c.complemento,
                c.tipo,
                c.vinculo
            FROM tb_colaborador c
            WHERE c.id_colaborador = @id;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(command, "@id", id);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return await MapAsync(
            reader,
            connection,
            cancellationToken);
    }

    public async Task<IEnumerable<Colaborador>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var colaboradores = new List<Colaborador>();

        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                c.id_colaborador,
                c.nome,
                c.cpf,
                c.email,
                c.telefone,
                c.logradouro_id,
                c.numero,
                c.complemento,
                c.tipo,
                c.vinculo
            FROM tb_colaborador c
            ORDER BY c.id_colaborador;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var colaborador =
                await MapAsync(
                    reader,
                    connection,
                    cancellationToken);

            if (colaborador != null)
                colaboradores.Add(colaborador);
        }

        return colaboradores;
    }

    public async Task<Colaborador> Adicionar(
        Colaborador colaborador,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        throw new NotSupportedException(
            "O Colaborador possui Endereco sem ID de Logradouro. " +
            "É necessário informar o ID do Logradouro antes da persistência.");
    }

    public async Task<Colaborador> Atualizar(
        Colaborador colaborador,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        throw new NotSupportedException(
            "O Colaborador possui Endereco sem ID de Logradouro. " +
            "É necessário informar o ID do Logradouro antes da persistência.");
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            DELETE FROM tb_colaborador
            WHERE id_colaborador = @id;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(
            command,
            "@id",
            id);

        var resultado =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        return resultado > 0;
    }

    public async Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        var sql = """
            SELECT COUNT(1)
            FROM tb_colaborador
            WHERE cpf = @cpf
            """;

        if (id.HasValue)
            sql += " AND id_colaborador <> @id";

        sql += ";";

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(
            command,
            "@cpf",
            cpf.Valor);

        if (id.HasValue)
        {
            AdicionarParametro(
                command,
                "@id",
                id.Value);
        }

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado) > 0;
    }

    public async Task<bool> EmailJaExiste(
        Email email,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        var sql = """
            SELECT COUNT(1)
            FROM tb_colaborador
            WHERE email = @email
            """;

        if (id.HasValue)
            sql += " AND id_colaborador <> @id";

        sql += ";";

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(
            command,
            "@email",
            email.Valor);

        if (id.HasValue)
        {
            AdicionarParametro(
                command,
                "@id",
                id.Value);
        }

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado) > 0;
    }

    public async Task<Colaborador?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                c.id_colaborador,
                c.nome,
                c.cpf,
                c.email,
                c.telefone,
                c.logradouro_id,
                c.numero,
                c.complemento,
                c.tipo,
                c.vinculo
            FROM tb_colaborador c
            WHERE c.cpf = @cpf;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(command, "@cpf", cpf.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return await MapAsync(
            reader,
            connection,
            cancellationToken);
    }

    public async Task<Colaborador?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                c.id_colaborador,
                c.nome,
                c.cpf,
                c.email,
                c.telefone,
                c.logradouro_id,
                c.numero,
                c.complemento,
                c.tipo,
                c.vinculo
            FROM tb_colaborador c
            WHERE c.email = @email;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(command, "@email", email.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return await MapAsync(
            reader,
            connection,
            cancellationToken);
    }

    public async Task<IEnumerable<Colaborador>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = new List<Colaborador>();

        using var connection = CriarConexao();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                c.id_colaborador,
                c.nome,
                c.cpf,
                c.email,
                c.telefone,
                c.logradouro_id,
                c.numero,
                c.complemento,
                c.tipo,
                c.vinculo
            FROM tb_colaborador c
            WHERE c.nome LIKE @nome
            ORDER BY c.nome;
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AdicionarParametro(
            command,
            "@nome",
            $"%{nome}%");

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var colaborador =
                await MapAsync(
                    reader,
                    connection,
                    cancellationToken);

            if (colaborador != null)
                colaboradores.Add(colaborador);
        }

        return colaboradores;
    }

    public async Task<bool> TrocarSenha(
        int id,
        Senha novaSenha,
        CancellationToken cancellationToken = default)
    {
        return false;
    }

    private async Task<Colaborador?> MapAsync(
        System.Data.Common.DbDataReader reader,
        System.Data.Common.DbConnection connection,
        CancellationToken cancellationToken)
    {
        var id =
            Convert.ToInt32(reader["id_colaborador"]);

        var nome =
            reader["nome"]?.ToString() ?? string.Empty;

        var cpfTexto =
            reader["cpf"]?.ToString() ?? string.Empty;

        var emailTexto =
            reader["email"]?.ToString() ?? string.Empty;

        var telefoneTexto =
            reader["telefone"]?.ToString() ?? string.Empty;

        var logradouroId =
            Convert.ToInt32(reader["logradouro_id"]);

        var numero =
            reader["numero"]?.ToString() ?? string.Empty;

        var complemento =
            reader["complemento"]?.ToString() ?? string.Empty;

        var tipo =
            (EColaboradorTipo)Convert.ToInt32(
                reader["tipo"]);

        var vinculo =
            (EColaboradorVinculo)Convert.ToInt32(
                reader["vinculo"]);

        const string sqlLogradouro = """
            SELECT
                id_logradouro,
                cep,
                nome,
                numero,
                bairro,
                cidade,
                estado
            FROM tb_logradouro
            WHERE id_logradouro = @id;
            """;

        using var commandLogradouro =
            connection.CreateCommand();

        commandLogradouro.CommandText =
            sqlLogradouro;

        AdicionarParametro(
            commandLogradouro,
            "@id",
            logradouroId);

        using var logradouroReader =
            await commandLogradouro.ExecuteReaderAsync(
                cancellationToken);

        if (!await logradouroReader.ReadAsync(
                cancellationToken))
        {
            return null;
        }

        var enderecoResult =
            Endereco.Criar(
                logradouroReader["cep"]?.ToString()
                    ?? string.Empty,
                logradouroReader["nome"]?.ToString()
                    ?? string.Empty,
                numero,
                complemento,
                logradouroReader["bairro"]?.ToString()
                    ?? string.Empty,
                logradouroReader["cidade"]?.ToString()
                    ?? string.Empty,
                logradouroReader["estado"]?.ToString()
                    ?? string.Empty);

        if (enderecoResult.IsFailure)
            return null;

        var cpfResult = Cpf.Criar(cpfTexto);

        if (cpfResult.IsFailure)
            return null;

        var emailResult = Email.Criar(emailTexto);

        if (emailResult.IsFailure)
            return null;

        var telefoneResult = Telefone.Criar(telefoneTexto);

        if (telefoneResult.IsFailure)
            return null;

        var colaboradorResult =
            Colaborador.Criar(
                nome,
                cpfResult.Value!,
                emailResult.Value!,
                telefoneResult.Value!,
                enderecoResult.Value!,
                tipo,
                vinculo);

        if (colaboradorResult.IsFailure)
            return null;

        return colaboradorResult.Value!;
    }

    private static void AdicionarParametro(
        System.Data.Common.DbCommand command,
        string nome,
        object? valor)
    {
        var parametro = command.CreateParameter();

        parametro.ParameterName = nome;
        parametro.Value = valor ?? DBNull.Value;

        command.Parameters.Add(parametro);
    }
}

