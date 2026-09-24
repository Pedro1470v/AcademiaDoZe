// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Data;

public class LogradouroRepository : BaseRepository, ILogradouroRepository
{
    public LogradouroRepository(string connectionString)
        : base(connectionString, DatabaseType.Sqlite)
    {
    }

    public async Task<Logradouro> Adicionar(
        Logradouro logradouro,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO tb_logradouro
            (
                cep,
                nome,
                numero,
                bairro,
                cidade,
                estado
            )
            VALUES
            (
                @cep,
                @nome,
                @numero,
                @bairro,
                @cidade,
                @estado
            );

            SELECT last_insert_rowid();
        ";

        var pCep = command.CreateParameter();
        pCep.ParameterName = "@cep";
        pCep.Value = logradouro.Cep;
        command.Parameters.Add(pCep);

        var pNome = command.CreateParameter();
        pNome.ParameterName = "@nome";
        pNome.Value = logradouro.Nome;
        command.Parameters.Add(pNome);

        var pNumero = command.CreateParameter();
        pNumero.ParameterName = "@numero";
        pNumero.Value = logradouro.Numero;
        command.Parameters.Add(pNumero);

        var pBairro = command.CreateParameter();
        pBairro.ParameterName = "@bairro";
        pBairro.Value = logradouro.Bairro;
        command.Parameters.Add(pBairro);

        var pCidade = command.CreateParameter();
        pCidade.ParameterName = "@cidade";
        pCidade.Value = logradouro.Cidade;
        command.Parameters.Add(pCidade);

        var pEstado = command.CreateParameter();
        pEstado.ParameterName = "@estado";
        pEstado.Value = logradouro.Estado;
        command.Parameters.Add(pEstado);

        var resultado =
            await command.ExecuteScalarAsync(cancellationToken);

        var id = Convert.ToInt32(resultado);

        var logradouroResult = Logradouro.Criar(
            id,
            logradouro.Cep,
            logradouro.Nome,
            logradouro.Numero,
            logradouro.Bairro,
            logradouro.Cidade,
            logradouro.Estado);

        if (logradouroResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível criar o logradouro após a inserção.");
        }

        return logradouroResult.Value!;
    }

    public async Task<Logradouro?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
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
        ";

        var parametro = command.CreateParameter();
        parametro.ParameterName = "@id";
        parametro.Value = id;

        command.Parameters.Add(parametro);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var resultado = Logradouro.Criar(
            Convert.ToInt32(reader["id_logradouro"]),
            reader["cep"].ToString()!,
            reader["nome"].ToString()!,
            reader["numero"].ToString()!,
            reader["bairro"].ToString()!,
            reader["cidade"].ToString()!,
            reader["estado"].ToString()!);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível reconstruir o logradouro.");
        }

        return resultado.Value!;
    }

    public async Task<IEnumerable<Logradouro>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var lista = new List<Logradouro>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                id_logradouro,
                cep,
                nome,
                numero,
                bairro,
                cidade,
                estado
            FROM tb_logradouro
            ORDER BY id_logradouro;
        ";

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var resultado = Logradouro.Criar(
                Convert.ToInt32(reader["id_logradouro"]),
                reader["cep"].ToString()!,
                reader["nome"].ToString()!,
                reader["numero"].ToString()!,
                reader["bairro"].ToString()!,
                reader["cidade"].ToString()!,
                reader["estado"].ToString()!);

            if (resultado.IsSuccess)
            {
                lista.Add(resultado.Value!);
            }
        }

        return lista;
    }

    public async Task<Logradouro> Atualizar(
        Logradouro logradouro,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE tb_logradouro
            SET
                cep = @cep,
                nome = @nome,
                numero = @numero,
                bairro = @bairro,
                cidade = @cidade,
                estado = @estado
            WHERE id_logradouro = @id;
        ";

        var pId = command.CreateParameter();
        pId.ParameterName = "@id";
        pId.Value = logradouro.ID;
        command.Parameters.Add(pId);

        var pCep = command.CreateParameter();
        pCep.ParameterName = "@cep";
        pCep.Value = logradouro.Cep;
        command.Parameters.Add(pCep);

        var pNome = command.CreateParameter();
        pNome.ParameterName = "@nome";
        pNome.Value = logradouro.Nome;
        command.Parameters.Add(pNome);

        var pNumero = command.CreateParameter();
        pNumero.ParameterName = "@numero";
        pNumero.Value = logradouro.Numero;
        command.Parameters.Add(pNumero);

        var pBairro = command.CreateParameter();
        pBairro.ParameterName = "@bairro";
        pBairro.Value = logradouro.Bairro;
        command.Parameters.Add(pBairro);

        var pCidade = command.CreateParameter();
        pCidade.ParameterName = "@cidade";
        pCidade.Value = logradouro.Cidade;
        command.Parameters.Add(pCidade);

        var pEstado = command.CreateParameter();
        pEstado.ParameterName = "@estado";
        pEstado.Value = logradouro.Estado;
        command.Parameters.Add(pEstado);

        var linhas =
            await command.ExecuteNonQueryAsync(cancellationToken);

        if (linhas == 0)
        {
            throw new InvalidOperationException(
                $"Nenhum logradouro encontrado com ID {logradouro.ID}.");
        }

        return logradouro;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM tb_logradouro
            WHERE id_logradouro = @id;
        ";

        var parametro = command.CreateParameter();
        parametro.ParameterName = "@id";
        parametro.Value = id;

        command.Parameters.Add(parametro);

        var linhas =
            await command.ExecuteNonQueryAsync(cancellationToken);

        return linhas > 0;
    }
}