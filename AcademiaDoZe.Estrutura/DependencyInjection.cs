using Microsoft.Extensions.DependencyInjection;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<IAlunoRepository>(
            _ => new AlunoRepository(connectionString));

        services.AddScoped<IColaboradorRepository>(
            _ => new ColaboradorRepository(connectionString));

        services.AddScoped<IAcessoAlunoRepository>(
            _ => new AcessoAlunoRepository(connectionString));

        services.AddScoped<IAcessoColaboradorRepository>(
            _ => new AcessoColaboradorRepository(connectionString));

        services.AddScoped<ILogradouroRepository>(
            _ => new LogradouroRepository(connectionString));

        services.AddScoped<IMatriculaRepository>(
            _ => new MatriculaRepository(connectionString));

        return services;
    }
}