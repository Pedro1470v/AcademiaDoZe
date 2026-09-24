// Pedro Vinicius

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class TelefoneRepository : BaseRepository
{
    public TelefoneRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Telefone telefone)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Telefones
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}