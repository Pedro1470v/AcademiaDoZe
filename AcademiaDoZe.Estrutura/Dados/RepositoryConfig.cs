namespace AcademiaDoZe.Infrastructure.Data;

public class RepositoryConfig
{
    public string ConnectionString { get; }

    public RepositoryConfig(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "A string de conexão não pode ser vazia.",
                nameof(connectionString));
        }

        ConnectionString = connectionString;
    }
}