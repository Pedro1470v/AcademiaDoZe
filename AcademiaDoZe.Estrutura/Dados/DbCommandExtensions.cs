//Pedro Vinicius 
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbCommandExtensions
{
    public static void AddParameter(
        this DbCommand command,
        string name,
        object? value,
        DbType dbType)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.DbType = dbType;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }
}