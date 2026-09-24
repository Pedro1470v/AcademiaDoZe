// Pedro Vinicius

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class EmailRepository : BaseRepository
{
    public EmailRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Email email)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Emails
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}