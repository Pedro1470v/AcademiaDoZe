
// Pedro Vinicius

using Microsoft.Data.Sqlite;

namespace AcademiaDoZe.Infrastructure.Data;

public class DbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "A string de conexão não pode ser vazia.",
                nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task InicializarAsync()
    {
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = """
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS tb_logradouro
            (
                id_logradouro INTEGER PRIMARY KEY AUTOINCREMENT,
                cep TEXT NOT NULL,
                nome TEXT NOT NULL,
                numero TEXT,
                bairro TEXT NOT NULL,
                cidade TEXT NOT NULL,
                estado TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS tb_colaborador
            (
                id_colaborador INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                cpf TEXT NOT NULL,
                email TEXT NOT NULL,
                telefone TEXT NOT NULL,
                logradouro_id INTEGER NOT NULL,
                numero TEXT NOT NULL,
                complemento TEXT,
                nascimento TEXT NOT NULL DEFAULT '',
                admissao TEXT NOT NULL DEFAULT '',
                senha TEXT NOT NULL DEFAULT '',
                foto BLOB,
                tipo INTEGER NOT NULL,
                vinculo INTEGER NOT NULL,

                FOREIGN KEY (logradouro_id)
                    REFERENCES tb_logradouro(id_logradouro)
            );

            CREATE TABLE IF NOT EXISTS tb_aluno
            (
                id_aluno INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                cpf TEXT NOT NULL,
                nascimento TEXT NOT NULL,
                telefone TEXT NOT NULL,
                email TEXT NOT NULL,
                logradouro_id INTEGER NOT NULL,
                numero TEXT NOT NULL,
                complemento TEXT,
                senha TEXT NOT NULL,
                foto BLOB,

                FOREIGN KEY (logradouro_id)
                    REFERENCES tb_logradouro(id_logradouro)
            );

            CREATE TABLE IF NOT EXISTS tb_matricula
            (
                id_matricula INTEGER PRIMARY KEY AUTOINCREMENT,
                aluno_id INTEGER NOT NULL,
                plano INTEGER NOT NULL,
                data_inicio TEXT NOT NULL,
                data_fim TEXT NOT NULL,
                objetivo TEXT NOT NULL,
                restricao_medica INTEGER NOT NULL,
                obs_restricao TEXT,
                laudo_medico BLOB,

                FOREIGN KEY (aluno_id)
                    REFERENCES tb_aluno(id_aluno)
            );

            CREATE TABLE IF NOT EXISTS tb_acesso_aluno
            (
                id_acesso_aluno INTEGER PRIMARY KEY AUTOINCREMENT,
                aluno_id INTEGER NOT NULL,
                data_hora_entrada TEXT NOT NULL,
                data_hora_saida TEXT,

                FOREIGN KEY (aluno_id)
                    REFERENCES tb_aluno(id_aluno)
            );

            CREATE TABLE IF NOT EXISTS tb_acesso_colaborador
            (
                id_acesso_colaborador INTEGER PRIMARY KEY AUTOINCREMENT,
                colaborador_id INTEGER NOT NULL,
                data_hora_entrada TEXT NOT NULL,
                data_hora_saida TEXT,

                FOREIGN KEY (colaborador_id)
                    REFERENCES tb_colaborador(id_colaborador)
            );
            """;

        await command.ExecuteNonQueryAsync();

        // Atualiza tabelas existentes sem apagar seus registros.
        await GarantirColunaAsync(
            connection,
            "tb_colaborador",
            "nascimento",
            "TEXT NOT NULL DEFAULT ''");

        await GarantirColunaAsync(
            connection,
            "tb_colaborador",
            "admissao",
            "TEXT NOT NULL DEFAULT ''");

        await GarantirColunaAsync(
            connection,
            "tb_colaborador",
            "senha",
            "TEXT NOT NULL DEFAULT ''");

        await GarantirColunaAsync(
            connection,
            "tb_colaborador",
            "foto",
            "BLOB");
    }

    private static async Task GarantirColunaAsync(
        SqliteConnection connection,
        string tabela,
        string coluna,
        string definicao)
    {
        await using var command = connection.CreateCommand();

        command.CommandText = $"PRAGMA table_info({tabela});";

        await using var reader =
            await command.ExecuteReaderAsync();

        var existe = false;

        while (await reader.ReadAsync())
        {
            var nomeColuna = reader["name"]?.ToString();

            if (string.Equals(
                nomeColuna,
                coluna,
                StringComparison.OrdinalIgnoreCase))
            {
                existe = true;
                break;
            }
        }

        await reader.DisposeAsync();

        if (existe)
            return;

        await using var alterCommand = connection.CreateCommand();

        alterCommand.CommandText =
            $"ALTER TABLE {tabela} ADD COLUMN {coluna} {definicao};";

        await alterCommand.ExecuteNonQueryAsync();
    }
}
