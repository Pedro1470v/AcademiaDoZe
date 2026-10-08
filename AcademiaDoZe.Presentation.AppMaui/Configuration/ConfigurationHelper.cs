using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var dbPath = @"C:\Users\Usuário\source\repos\AcademiaDoZe\AcademiaDoZe.Infrastructure.Testes\db_academia_do_ze.db";

        var connectionString = $"Data Source={dbPath};Default Timeout=5;";

        services.AddSingleton(new RepositoryConfig(connectionString));

        services.AddApplicationServices();
    }
}