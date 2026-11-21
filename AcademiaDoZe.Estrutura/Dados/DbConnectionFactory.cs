//Pedro Vinicius
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbConnectionFactory
{
    public static DbConnection CreateConnection(
        string connectionString,
        DatabaseType databaseType)
    {
        var factory = DbProvider.GetFactory(databaseType);

        var connection = factory.CreateConnection()
            ?? throw new InvalidOperationException(
                "Não foi possível criar a conexão com o banco de dados.");

        connection.ConnectionString = connectionString;

        return connection;
    }
}