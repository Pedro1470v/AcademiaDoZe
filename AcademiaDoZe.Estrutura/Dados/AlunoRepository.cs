// Pedro Vinicius

using System.Data.Common;
using System.Linq;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class AlunoRepository : BaseRepository, IAlunoRepository
{
    public AlunoRepository(string connectionString)
        : base(connectionString, DatabaseType.Sqlite)
    {
    }

    public async Task<Aluno?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                a.id_aluno,
                a.nome,
                a.cpf,
                a.nascimento,
                a.telefone,
                a.email,
                a.logradouro_id,
                a.numero,
                a.complemento,
                a.senha,
                a.foto,
                l.cep,
                l.nome AS logradouro_nome,
                l.bairro,
                l.cidade,
                l.estado
            FROM tb_aluno a
            INNER JOIN tb_logradouro l
                ON a.logradouro_id = l.id_logradouro
            WHERE a.id_aluno = @id;
        ";

        AdicionarParametro(command, "@id", id);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    public async Task<IEnumerable<Aluno>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        var alunos = new List<Aluno>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                a.id_aluno,
                a.nome,
                a.cpf,
                a.nascimento,
                a.telefone,
                a.email,
                a.logradouro_id,
                a.numero,
                a.complemento,
                a.senha,
                a.foto,
                l.cep,
                l.nome AS logradouro_nome,
                l.bairro,
                l.cidade,
                l.estado
            FROM tb_aluno a
            INNER JOIN tb_logradouro l
                ON a.logradouro_id = l.id_logradouro
            ORDER BY a.nome;
        ";

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            alunos.Add(Map(reader));
        }

        return alunos;
    }

    public async Task<Aluno> Adicionar(
        Aluno entity,
        CancellationToken cancellationToken = default)
    {
        var id = await InserirAsync(
            entity,
            cancellationToken);

        var aluno = await ObterPorId(
            id,
            cancellationToken);

        if (aluno == null)
        {
            throw new InvalidOperationException(
                "O aluno foi inserido, mas não foi possível recuperá-lo.");
        }

        return aluno;
    }

    public async Task<Aluno> Atualizar(
        Aluno entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        var logradouroId =
            await ObterIdLogradouroAsync(
                connection,
                entity.Logradouro.Cep,
                cancellationToken);

        if (logradouroId == null)
        {
            throw new InvalidOperationException(
                "O logradouro do aluno não foi encontrado no banco.");
        }

        using var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE tb_aluno
            SET
                nome = @nome,
                cpf = @cpf,
                nascimento = @nascimento,
                telefone = @telefone,
                email = @email,
                logradouro_id = @logradouro_id,
                numero = @numero,
                complemento = @complemento,
                senha = @senha,
                foto = @foto
            WHERE id_aluno = @id;
        ";

        AdicionarParametro(command, "@id", entity.ID);
        AdicionarParametro(command, "@nome", entity.Nome);
        AdicionarParametro(command, "@cpf", entity.Cpf.Valor);

        AdicionarParametro(
            command,
            "@nascimento",
            entity.DataNascimento.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@telefone",
            entity.Telefone.Valor);

        AdicionarParametro(
            command,
            "@email",
            entity.Email.Valor);

        AdicionarParametro(
            command,
            "@logradouro_id",
            logradouroId.Value);

        AdicionarParametro(
            command,
            "@numero",
            entity.Numero);

        AdicionarParametro(
            command,
            "@complemento",
            string.IsNullOrWhiteSpace(entity.Complemento)
                ? DBNull.Value
                : entity.Complemento);

        AdicionarParametro(
            command,
            "@senha",
            entity.Senha.Valor);

        AdicionarParametro(
            command,
            "@foto",
            entity.Foto?.Conteudo ?? (object)DBNull.Value);

        var linhas =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        if (linhas == 0)
        {
            throw new InvalidOperationException(
                $"Nenhum aluno encontrado com ID {entity.ID}.");
        }

        var alunoAtualizado =
            await ObterPorId(
                entity.ID,
                cancellationToken);

        if (alunoAtualizado == null)
        {
            throw new InvalidOperationException(
                "O aluno foi atualizado, mas não foi possível recuperá-lo.");
        }

        return alunoAtualizado;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM tb_aluno
            WHERE id_aluno = @id;
        ";

        AdicionarParametro(command, "@id", id);

        var linhas =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        return linhas > 0;
    }

    public async Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = id.HasValue
            ? @"
                SELECT COUNT(1)
                FROM tb_aluno
                WHERE cpf = @cpf
                  AND id_aluno <> @id;
              "
            : @"
                SELECT COUNT(1)
                FROM tb_aluno
                WHERE cpf = @cpf;
              ";

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

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = id.HasValue
            ? @"
                SELECT COUNT(1)
                FROM tb_aluno
                WHERE email = @email
                  AND id_aluno <> @id;
              "
            : @"
                SELECT COUNT(1)
                FROM tb_aluno
                WHERE email = @email;
              ";

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

    public async Task<Aluno?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                a.id_aluno,
                a.nome,
                a.cpf,
                a.nascimento,
                a.telefone,
                a.email,
                a.logradouro_id,
                a.numero,
                a.complemento,
                a.senha,
                a.foto,
                l.cep,
                l.nome AS logradouro_nome,
                l.bairro,
                l.cidade,
                l.estado
            FROM tb_aluno a
            INNER JOIN tb_logradouro l
                ON a.logradouro_id = l.id_logradouro
            WHERE a.cpf = @cpf
            LIMIT 1;
        ";

        AdicionarParametro(
            command,
            "@cpf",
            cpf.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    public async Task<Aluno?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                a.id_aluno,
                a.nome,
                a.cpf,
                a.nascimento,
                a.telefone,
                a.email,
                a.logradouro_id,
                a.numero,
                a.complemento,
                a.senha,
                a.foto,
                l.cep,
                l.nome AS logradouro_nome,
                l.bairro,
                l.cidade,
                l.estado
            FROM tb_aluno a
            INNER JOIN tb_logradouro l
                ON a.logradouro_id = l.id_logradouro
            WHERE a.email = @email
            LIMIT 1;
        ";

        AdicionarParametro(
            command,
            "@email",
            email.Valor);

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return Map(reader);
    }

    public async Task<IEnumerable<Aluno>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default)
    {
        var alunos = new List<Aluno>();

        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                a.id_aluno,
                a.nome,
                a.cpf,
                a.nascimento,
                a.telefone,
                a.email,
                a.logradouro_id,
                a.numero,
                a.complemento,
                a.senha,
                a.foto,
                l.cep,
                l.nome AS logradouro_nome,
                l.bairro,
                l.cidade,
                l.estado
            FROM tb_aluno a
            INNER JOIN tb_logradouro l
                ON a.logradouro_id = l.id_logradouro
            WHERE a.nome LIKE @nome
            ORDER BY a.nome;
        ";

        AdicionarParametro(
            command,
            "@nome",
            $"%{nome.Trim()}%");

        using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            alunos.Add(Map(reader));
        }

        return alunos;
    }

    public async Task<bool> TrocarSenha(
        int id,
        Senha novaSenha,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE tb_aluno
            SET senha = @senha
            WHERE id_aluno = @id;
        ";

        AdicionarParametro(
            command,
            "@id",
            id);

        AdicionarParametro(
            command,
            "@senha",
            novaSenha.Valor);

        var linhas =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        return linhas > 0;
    }

    public async Task<int> InserirAsync(
        Aluno aluno,
        CancellationToken cancellationToken = default)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        var logradouroId =
            await ObterIdLogradouroAsync(
                connection,
                aluno.Logradouro.Cep,
                cancellationToken);

        if (logradouroId == null)
        {
            throw new InvalidOperationException(
                "O logradouro do aluno não foi encontrado no banco.");
        }

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO tb_aluno
            (
                nome,
                cpf,
                nascimento,
                telefone,
                email,
                logradouro_id,
                numero,
                complemento,
                senha,
                foto
            )
            VALUES
            (
                @nome,
                @cpf,
                @nascimento,
                @telefone,
                @email,
                @logradouro_id,
                @numero,
                @complemento,
                @senha,
                @foto
            );

            SELECT last_insert_rowid();
        ";

        AdicionarParametro(
            command,
            "@nome",
            aluno.Nome);

        AdicionarParametro(
            command,
            "@cpf",
            aluno.Cpf.Valor);

        AdicionarParametro(
            command,
            "@nascimento",
            aluno.DataNascimento.ToString("yyyy-MM-dd"));

        AdicionarParametro(
            command,
            "@telefone",
            aluno.Telefone.Valor);

        AdicionarParametro(
            command,
            "@email",
            aluno.Email.Valor);

        AdicionarParametro(
            command,
            "@logradouro_id",
            logradouroId.Value);

        AdicionarParametro(
            command,
            "@numero",
            aluno.Numero);

        AdicionarParametro(
            command,
            "@complemento",
            string.IsNullOrWhiteSpace(aluno.Complemento)
                ? DBNull.Value
                : aluno.Complemento);

        AdicionarParametro(
            command,
            "@senha",
            aluno.Senha.Valor);

        AdicionarParametro(
            command,
            "@foto",
            aluno.Foto?.Conteudo ?? (object)DBNull.Value);

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado);
    }

    private async Task<int?> ObterIdLogradouroAsync(
        DbConnection connection,
        string cep,
        CancellationToken cancellationToken)
    {
        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT id_logradouro
            FROM tb_logradouro
            WHERE cep = @cep
            LIMIT 1;
        ";

        AdicionarParametro(
            command,
            "@cep",
            cep);

        var resultado =
            await command.ExecuteScalarAsync(
                cancellationToken);

        if (resultado == null ||
            resultado == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(resultado);
    }

    private static void AdicionarParametro(
        DbCommand command,
        string nome,
        object? valor)
    {
        var parametro = command.CreateParameter();

        parametro.ParameterName = nome;
        parametro.Value = valor ?? DBNull.Value;

        command.Parameters.Add(parametro);
    }

    private static Aluno Map(
        DbDataReader reader)
    {
        var id =
            Convert.ToInt32(reader["id_aluno"]);

        var nome =
            Convert.ToString(reader["nome"])
            ?? string.Empty;

        var cpf =
            Convert.ToString(reader["cpf"])
            ?? string.Empty;

        var nascimentoTexto =
            Convert.ToString(reader["nascimento"])
            ?? string.Empty;

        var dataNascimento =
            DateOnly.Parse(nascimentoTexto);

        var telefone =
            Convert.ToString(reader["telefone"])
            ?? string.Empty;

        var email =
            Convert.ToString(reader["email"])
            ?? string.Empty;

        var cep =
            Convert.ToString(reader["cep"])
            ?? string.Empty;

        var nomeLogradouro =
            Convert.ToString(reader["logradouro_nome"])
            ?? string.Empty;

        var bairro =
            Convert.ToString(reader["bairro"])
            ?? string.Empty;

        var cidade =
            Convert.ToString(reader["cidade"])
            ?? string.Empty;

        var estado =
            Convert.ToString(reader["estado"])
            ?? string.Empty;

        var numero =
            Convert.ToString(reader["numero"])
            ?? string.Empty;

        var complemento =
            reader["complemento"] == DBNull.Value
                ? string.Empty
                : Convert.ToString(reader["complemento"])
                    ?? string.Empty;

        var senha =
            Convert.ToString(reader["senha"])
            ?? string.Empty;

        byte[] fotoBytes;

        if (reader["foto"] == DBNull.Value)
        {
            fotoBytes = Array.Empty<byte>();
        }
        else
        {
            fotoBytes = (byte[])reader["foto"];
        }

        var logradouroResult =
            Logradouro.Criar(
                cep,
                nomeLogradouro,
                bairro,
                cidade,
                estado);

        if (logradouroResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro ao reconstruir o logradouro do aluno.");
        }

        var fotoResult =
            Arquivo.Criar(fotoBytes);

        if (fotoResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro ao reconstruir a foto do aluno.");
        }

        var alunoResult =
            Aluno.Criar(
                id,
                nome,
                cpf,
                dataNascimento,
                telefone,
                email,
                logradouroResult.Value!,
                numero,
                complemento,
                senha,
                fotoResult.Value!);

        if (alunoResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro ao reconstruir o aluno: " +
                string.Join(
                    ", ",
                    alunoResult.Notifications
                        .Select(n => n.Mensagem)));
        }

        return alunoResult.Value!;
    }
}