// Pedro Vinicius

using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(
            new RepositoryConfig(connectionString));

        services.AddTransient<IAlunoRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new AlunoRepository(
                    config.ConnectionString);
            });

        services.AddTransient<IColaboradorRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new ColaboradorRepository(
                    config.ConnectionString);
            });

        services.AddTransient<ILogradouroRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new LogradouroRepository(
                    config.ConnectionString);
            });

        services.AddTransient<IMatriculaRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new MatriculaRepository(
                    config.ConnectionString);
            });

        services.AddTransient<IAcessoAlunoRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new AcessoAlunoRepository(
                    config.ConnectionString);
            });

        services.AddTransient<IAcessoColaboradorRepository>(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return new AcessoColaboradorRepository(
                    config.ConnectionString);
            });

        return services;
    }
}