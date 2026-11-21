
// Pedro Vinicius

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
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

    private const string SqlSelect = """
        SELECT
            c.id_colaborador,
            c.cpf,
            c.nome,
            c.nascimento,
            c.telefone,
            c.email,
            l.cep,
            l.nome AS logradouro,
            c.numero,
            c.complemento,
            l.bairro,
            l.cidade,
            l.estado,
            c.senha,
            c.foto,
            c.admissao,
            c.tipo,
            c.vinculo
        FROM tb_colaborador c
        INNER JOIN tb_logradouro l
            ON l.id_logradouro = c.logradouro_id
        """;

    public async Task<Colaborador?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = SqlSelect +
            " WHERE c.id_colaborador = @id;";

        AdicionarParametro(command, "@id", id);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return MapAsync(reader);
    }

    public async Task<IEnumerable<Colaborador>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var colaboradores = new List<Colaborador>();

        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = SqlSelect +
            " ORDER BY c.id_colaborador;";

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var colaborador = MapAsync(reader);

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

        using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var logradouroId = await ObterLogradouroIdAsync(
                connection,
                transaction,
                colaborador.Endereco,
                cancellationToken);

            const string sql = """
                INSERT INTO tb_colaborador
                (
                    cpf,
                    nome,
                    nascimento,
                    telefone,
                    email,
                    logradouro_id,
                    numero,
                    complemento,
                    senha,
                    foto,
                    admissao,
                    tipo,
                    vinculo
                )
                VALUES
                (
                    @cpf,
                    @nome,
                    @nascimento,
                    @telefone,
                    @email,
                    @logradouro_id,
                    @numero,
                    @complemento,
                    @senha,
                    @foto,
                    @admissao,
                    @tipo,
                    @vinculo
                );

                SELECT last_insert_rowid();
                """;

            using var command = connection.CreateCommand();

            command.Transaction = transaction;
            command.CommandText = sql;

            AdicionarParametrosColaborador(command, colaborador);

            AdicionarParametro(
                command,
                "@logradouro_id",
                logradouroId);

            var resultado =
                await command.ExecuteScalarAsync(cancellationToken);

            var id = Convert.ToInt32(resultado);

            await transaction.CommitAsync(cancellationToken);

            return CriarComNovoId(colaborador, id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Colaborador> Atualizar(
        Colaborador colaborador,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var logradouroId = await ObterLogradouroIdAsync(
                connection,
                transaction,
                colaborador.Endereco,
                cancellationToken);

            const string sql = """
                UPDATE tb_colaborador
                SET
                    cpf = @cpf,
                    nome = @nome,
                    nascimento = @nascimento,
                    telefone = @telefone,
                    email = @email,
                    logradouro_id = @logradouro_id,
                    numero = @numero,
                    complemento = @complemento,
                    senha = @senha,
                    foto = @foto,
                    admissao = @admissao,
                    tipo = @tipo,
                    vinculo = @vinculo
                WHERE id_colaborador = @id;
                """;

            using var command = connection.CreateCommand();

            command.Transaction = transaction;
            command.CommandText = sql;

            AdicionarParametrosColaborador(command, colaborador);

            AdicionarParametro(
                command,
                "@logradouro_id",
                logradouroId);

            AdicionarParametro(
                command,
                "@id",
                colaborador.ID);

            var resultado =
                await command.ExecuteNonQueryAsync(cancellationToken);

            if (resultado == 0)
            {
                throw new InvalidOperationException(
                    "Colaborador não encontrado para atualização.");
            }

            await transaction.CommitAsync(cancellationToken);

            return colaborador;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = """
            DELETE FROM tb_colaborador
            WHERE id_colaborador = @id;
            """;

        AdicionarParametro(command, "@id", id);

        var resultado =
            await command.ExecuteNonQueryAsync(cancellationToken);

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

        using var command = connection.CreateCommand();
        command.CommandText = sql + ";";

        AdicionarParametro(command, "@cpf", cpf.Valor);

        if (id.HasValue)
            AdicionarParametro(command, "@id", id.Value);

        var resultado =
            await command.ExecuteScalarAsync(cancellationToken);

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

        using var command = connection.CreateCommand();
        command.CommandText = sql + ";";

        AdicionarParametro(command, "@email", email.Valor);

        if (id.HasValue)
            AdicionarParametro(command, "@id", id.Value);

        var resultado =
            await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(resultado) > 0;
    }

    public async Task<Colaborador?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = SqlSelect +
            " WHERE c.cpf = @cpf;";

        AdicionarParametro(command, "@cpf", cpf.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return MapAsync(reader);
    }

    public async Task<Colaborador?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = SqlSelect +
            " WHERE c.email = @email;";

        AdicionarParametro(command, "@email", email.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return MapAsync(reader);
    }

    public async Task<IEnumerable<Colaborador>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = new List<Colaborador>();

        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = SqlSelect +
            " WHERE c.nome LIKE @nome ORDER BY c.nome;";

        AdicionarParametro(command, "@nome", $"%{nome}%");

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var colaborador = MapAsync(reader);

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
        using var connection = CriarConexao();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE tb_colaborador
            SET senha = @senha
            WHERE id_colaborador = @id;
            """;

        AdicionarParametro(command, "@senha", novaSenha.Valor);
        AdicionarParametro(command, "@id", id);

        var resultado =
            await command.ExecuteNonQueryAsync(cancellationToken);

        return resultado > 0;
    }

    private static async Task<int> ObterLogradouroIdAsync(
        DbConnection connection,
        DbTransaction transaction,
        Endereco endereco,
        CancellationToken cancellationToken)
    {
        using var command = connection.CreateCommand();

        command.Transaction = transaction;

        command.CommandText = """
            SELECT id_logradouro
            FROM tb_logradouro
            WHERE cep = @cep
              AND nome = @logradouro
              AND bairro = @bairro
              AND cidade = @cidade
              AND estado = @estado
            ORDER BY id_logradouro
            LIMIT 1;
            """;

        AdicionarParametro(command, "@cep", endereco.Cep);
        AdicionarParametro(command, "@logradouro", endereco.Logradouro);
        AdicionarParametro(command, "@bairro", endereco.Bairro);
        AdicionarParametro(command, "@cidade", endereco.Cidade);
        AdicionarParametro(command, "@estado", endereco.Estado);

        var resultado =
            await command.ExecuteScalarAsync(cancellationToken);

        if (resultado == null || resultado == DBNull.Value)
        {
            throw new InvalidOperationException(
                "O logradouro informado não foi encontrado no banco de dados. " +
                "Consulte o CEP e selecione um logradouro cadastrado antes de salvar o colaborador.");
        }

        return Convert.ToInt32(resultado);
    }

    private static Colaborador? MapAsync(
        DbDataReader reader)
    {
        var id = Convert.ToInt32(reader["id_colaborador"]);

        var nome = reader["nome"]?.ToString() ?? string.Empty;
        var cpfTexto = reader["cpf"]?.ToString() ?? string.Empty;
        var emailTexto = reader["email"]?.ToString() ?? string.Empty;
        var telefoneTexto = reader["telefone"]?.ToString() ?? string.Empty;

        var nascimento = DateOnly.ParseExact(
            reader["nascimento"].ToString()!,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        var admissao = DateOnly.ParseExact(
            reader["admissao"].ToString()!,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        var enderecoResult = Endereco.Criar(
            reader["cep"]?.ToString() ?? string.Empty,
            reader["logradouro"]?.ToString() ?? string.Empty,
            reader["numero"]?.ToString() ?? string.Empty,
            reader["complemento"]?.ToString() ?? string.Empty,
            reader["bairro"]?.ToString() ?? string.Empty,
            reader["cidade"]?.ToString() ?? string.Empty,
            reader["estado"]?.ToString() ?? string.Empty);

        if (enderecoResult.IsFailure)
            return null;

        var cpfResult = Cpf.Criar(cpfTexto);
        var emailResult = Email.Criar(emailTexto);
        var telefoneResult = Telefone.Criar(telefoneTexto);

        var senhaResult = Senha.Criar(
            reader["senha"]?.ToString() ?? string.Empty);

        if (cpfResult.IsFailure ||
            emailResult.IsFailure ||
            telefoneResult.IsFailure ||
            senhaResult.IsFailure)
        {
            return null;
        }

        Arquivo? foto = null;

        if (reader["foto"] != DBNull.Value)
        {
            var conteudo = (byte[])reader["foto"];

            if (conteudo.Length > 0)
            {
                var fotoResult = Arquivo.Criar(conteudo);

                if (fotoResult.IsFailure)
                    return null;

                foto = fotoResult.Value;
            }
        }

        var tipo = (EColaboradorTipo)Convert.ToInt32(reader["tipo"]);

        var vinculo = (EColaboradorVinculo)Convert.ToInt32(reader["vinculo"]);

        var colaboradorResult = Colaborador.Criar(
            id,
            nome,
            cpfResult.Value!,
            emailResult.Value!,
            telefoneResult.Value!,
            enderecoResult.Value!,
            nascimento,
            admissao,
            senhaResult.Value!,
            foto,
            tipo,
            vinculo);

        return colaboradorResult.IsSuccess
            ? colaboradorResult.Value
            : null;
    }

    private static Colaborador CriarComNovoId(
        Colaborador colaborador,
        int id)
    {
        var resultado = Colaborador.Criar(
            id,
            colaborador.Nome,
            colaborador.Cpf,
            colaborador.Email,
            colaborador.Telefone,
            colaborador.Endereco,
            colaborador.DataNascimento,
            colaborador.DataAdmissao,
            colaborador.Senha,
            colaborador.Foto,
            colaborador.Tipo,
            colaborador.Vinculo);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível recuperar o colaborador cadastrado.");
        }

        return resultado.Value!;
    }

    private static void AdicionarParametrosColaborador(
        DbCommand command,
        Colaborador colaborador)
    {
        AdicionarParametro(command, "@cpf", colaborador.Cpf.Valor);
        AdicionarParametro(command, "@nome", colaborador.Nome);

        AdicionarParametro(
            command,
            "@nascimento",
            colaborador.DataNascimento.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture));

        AdicionarParametro(
            command,
            "@telefone",
            colaborador.Telefone.Valor);

        AdicionarParametro(
            command,
            "@email",
            colaborador.Email.Valor);

        AdicionarParametro(
            command,
            "@numero",
            colaborador.Endereco.Numero);

        AdicionarParametro(
            command,
            "@complemento",
            colaborador.Endereco.Complemento);

        AdicionarParametro(
            command,
            "@senha",
            colaborador.Senha.Valor);

        AdicionarParametro(
            command,
            "@foto",
            colaborador.Foto?.Conteudo);

        AdicionarParametro(
            command,
            "@admissao",
            colaborador.DataAdmissao.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture));

        AdicionarParametro(
            command,
            "@tipo",
            (int)colaborador.Tipo);

        AdicionarParametro(
            command,
            "@vinculo",
            (int)colaborador.Vinculo);
    }
}
