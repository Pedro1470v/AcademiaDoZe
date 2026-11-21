// Pedro Vinicius

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class CpfRepository : BaseRepository
{
    public CpfRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Cpf cpf)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Cpfs
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}