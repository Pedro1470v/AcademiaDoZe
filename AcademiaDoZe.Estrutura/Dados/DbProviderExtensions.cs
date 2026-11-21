//Pedro Vinicius 
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbProviderExtensions
{
    public static void AddParameter(
        this DbCommand command,
        string name,
        object? value)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }
}