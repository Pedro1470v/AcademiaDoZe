// Pedro Vinicius

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Data;

public class ArquivoRepository : BaseRepository
{
    public ArquivoRepository(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<int> InserirAsync(Arquivo arquivo)
    {
        using var connection = CriarConexao();

        await AbrirConexaoAsync(connection);

        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Arquivos
            DEFAULT VALUES;

            SELECT last_insert_rowid();
        ";

        var resultado = await command.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}