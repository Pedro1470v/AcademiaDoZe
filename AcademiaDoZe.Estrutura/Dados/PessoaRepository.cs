// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Data;

public class PessoaRepository : BaseRepository
{
    public PessoaRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Pessoa pessoa)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Pessoas
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}