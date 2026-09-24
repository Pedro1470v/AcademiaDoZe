using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace AcademiaDoZe.Infrastructure.Data;

public abstract class BaseRepository
{
    protected readonly string ConnectionString;
    protected readonly DatabaseType DatabaseType;

    protected BaseRepository(
        string connectionString,
        DatabaseType databaseType = DatabaseType.Sqlite)
    {
        ConnectionString = connectionString;
        DatabaseType = databaseType;
    }

    protected DbConnection CriarConexao()
    {
        return DatabaseType switch
        {
            DatabaseType.Sqlite =>
                new SqliteConnection(ConnectionString),

            DatabaseType.SqlServer =>
                new SqlConnection(ConnectionString),

            DatabaseType.MySql =>
                new MySqlConnection(ConnectionString),

            _ => throw new ArgumentOutOfRangeException(
                nameof(DatabaseType),
                DatabaseType,
                "Tipo de banco de dados não suportado.")
        };
    }

    protected async Task AbrirConexaoAsync(
        DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
    }

    protected static void AdicionarParametro(
        DbCommand command,
        string nome,
        object? valor)
    {
        var parametro = command.CreateParameter();

        parametro.ParameterName = nome;
        parametro.Value = valor ?? DBNull.Value;

        command.Parameters.Add(parametro);
    }
}