
// Pedro Vinicius

using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Serviços.
        services.AddTransient<ILogradouroService, LogradouroService>();
        services.AddTransient<IColaboradorService, ColaboradorService>();
        services.AddTransient<IAlunoService, AlunoService>();
        services.AddTransient<IMatriculaService, MatriculaService>();

        services.AddTransient<AcessoAlunoService>();
        services.AddTransient<AcessoColaboradorService>();

        // Fábricas de repositórios.
        services.AddTransient<Func<ILogradouroRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new LogradouroRepository(config.ConnectionString);
        });

        services.AddTransient<Func<IColaboradorRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new ColaboradorRepository(config.ConnectionString);
        });

        services.AddTransient<Func<IAlunoRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new AlunoRepository(config.ConnectionString);
        });

        services.AddTransient<Func<IMatriculaRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new MatriculaRepository(config.ConnectionString);
        });

        services.AddTransient<Func<IAcessoAlunoRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new AcessoAlunoRepository(config.ConnectionString);
        });

        services.AddTransient<Func<IAcessoColaboradorRepository>>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return () =>
                new AcessoColaboradorRepository(config.ConnectionString);
        });

        return services;
    }
}