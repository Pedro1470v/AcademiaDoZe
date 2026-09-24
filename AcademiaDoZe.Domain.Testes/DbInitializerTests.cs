// Pedro Vinicius

using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace AcademiaDoZe.Domain.Tests;

public class DbInitializerTests
{
    [Fact]
    public async Task Deve_Criar_Tabela_Alunos()
    {
        var connectionString = "Data Source=academiadoze_test.db";

        var initializer = new DbInitializer(connectionString);

        await initializer.InicializarAsync();

        await using var connection = new SqliteConnection(connectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
            AND name = 'Alunos';
        ";

        var resultado = await command.ExecuteScalarAsync();

        Assert.Equal("Alunos", resultado);
    }
}