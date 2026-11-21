// Pedro Vinicius

using AcademiaDoZe.Infrastructure.Data;

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerAssembly,
    DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests;

public abstract class TestBase
{
    private const DatabaseType SelectedDatabaseType =
        DatabaseType.Sqlite;

    protected string ConnectionString { get; }

    protected DatabaseType DatabaseType { get; }

    protected TestBase()
    {
        DatabaseType = SelectedDatabaseType;

        ConnectionString = DatabaseType switch
        {
            DatabaseType.SqlServer =>
                "Server=localhost\\SQLEXPRESS;Database=AcademiaDoZe;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;",

            DatabaseType.MySql =>
                "Server=localhost;Port=3306;Database=db_academia_do_ze;Uid=root;Pwd=SUA_SENHA;",

            DatabaseType.Sqlite =>
                "Data Source=C:\\Users\\Usuário\\source\\repos\\AcademiaDoZe\\AcademiaDoZe.Infrastructure.Testes\\db_academia_do_ze.db;Cache=Shared;",

            _ => throw new ArgumentOutOfRangeException(
                nameof(DatabaseType),
                DatabaseType,
                "SGBD não suportado para testes.")
        };

        var initializer = new DbInitializer(
            ConnectionString);

        initializer
            .InicializarAsync()
            .GetAwaiter()
            .GetResult();
    }

    #region Geradores de dados aleatórios

    private static int _counter = 10000;

    protected static string GerarCep()
    {
        return (
            80000000 +
            ((int)(DateTime.UtcNow.Ticks % 8000000)) +
            Interlocked.Increment(ref _counter)
        ).ToString("D8")[..8];
    }

    protected static string GerarCpf()
    {
        return (
            10000000000L +
            (DateTime.UtcNow.Ticks % 8000000000L) +
            Interlocked.Increment(ref _counter)
        ).ToString("D11")[..11];
    }

    protected static string GerarEmail()
    {
        return $"user_{Guid.NewGuid().ToString("N")[..8]}@test.com";
    }

    protected static string GerarTelefone()
    {
        return (
            49990000000L +
            (DateTime.UtcNow.Ticks % 8000000000L) +
            Interlocked.Increment(ref _counter)
        ).ToString("D11")[..11];
    }

    #endregion
}