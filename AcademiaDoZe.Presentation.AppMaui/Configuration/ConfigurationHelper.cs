
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System;
using System.IO;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    private const string ChaveCaminhoSqlite = "Sqlite_Caminho";
    private const string ChaveCaminhoSqliteLegado = "SqliteCaminho";
    private const string ChaveComplementoSqlite = "Sqlite_Complemento";

    public static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var (connectionString, _) = ObterConfiguracaoAtual();

        var repoConfig = new RepositoryConfig(connectionString);

        services.AddSingleton(repoConfig);

        services.AddSingleton<DbInitializer>(provider =>
            new DbInitializer(
                provider.GetRequiredService<RepositoryConfig>()
                    .ConnectionString));

        // Repositório de acesso dos alunos.
        services.AddScoped<IAcessoAlunoRepository>(provider =>
            new AcessoAlunoRepository(
                provider.GetRequiredService<RepositoryConfig>()
                    .ConnectionString));

        // Repositório de acesso dos colaboradores.
        services.AddScoped<IAcessoColaboradorRepository>(provider =>
            new AcessoColaboradorRepository(
                provider.GetRequiredService<RepositoryConfig>()
                    .ConnectionString));

        // Repositório de matrículas.
        services.AddScoped<IMatriculaRepository>(provider =>
            new MatriculaRepository(
                provider.GetRequiredService<RepositoryConfig>()
                    .ConnectionString));

        services.AddApplicationServices();
    }

    public static (string ConnectionString, string DatabaseType)
        ObterConfiguracaoAtual()
    {
        var diretorioAplicativo = FileSystem.AppDataDirectory;

        Directory.CreateDirectory(diretorioAplicativo);

        var caminhoPadrao = Path.Combine(
            diretorioAplicativo,
            "db_academia_do_ze.db");

        var caminhoBanco = Preferences.Get(
            ChaveCaminhoSqlite,
            Preferences.Get(
                ChaveCaminhoSqliteLegado,
                caminhoPadrao));

        if (string.IsNullOrWhiteSpace(caminhoBanco))
            caminhoBanco = caminhoPadrao;

        caminhoBanco = caminhoBanco.Trim();

        var diretorioBanco = Path.GetDirectoryName(caminhoBanco);

        if (!string.IsNullOrWhiteSpace(diretorioBanco))
            Directory.CreateDirectory(diretorioBanco);

        var complemento = Preferences.Get(
            ChaveComplementoSqlite,
            "Default Timeout=5;");

        if (string.IsNullOrWhiteSpace(complemento))
            complemento = "Default Timeout=5;";

        complemento = complemento.Trim();

        if (!complemento.EndsWith(";"))
            complemento += ";";

        var connectionString =
            $"Data Source={caminhoBanco};{complemento}";

        return (connectionString, "Sqlite");
    }
}
