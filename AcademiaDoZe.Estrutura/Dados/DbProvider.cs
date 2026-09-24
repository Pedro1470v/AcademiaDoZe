//Pedro Vinicius
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbProvider
{
    public static DbProviderFactory GetFactory(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => SqlClientFactory.Instance,
            DatabaseType.MySql => MySqlClientFactory.Instance,
            DatabaseType.Sqlite => SqliteFactory.Instance,

            _ => throw new ArgumentOutOfRangeException(
                nameof(databaseType),
                databaseType,
                "SGBD não suportado.")
        };
    }
}