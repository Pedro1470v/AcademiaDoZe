//Pedro Vinicius 
namespace AcademiaDoZe.Infrastructure.Exceptions;

public class RepositoryException : InfrastructureException
{
    public RepositoryException(string message)
        : base(message)
    {
    }

    public RepositoryException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}