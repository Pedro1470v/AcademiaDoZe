// Pedro Vinicius

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class EnderecoRepository : BaseRepository
{
    public EnderecoRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Endereco endereco)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Enderecos
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}