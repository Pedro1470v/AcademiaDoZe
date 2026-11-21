
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly DbInitializer _dbInitializer;

    public App(DbInitializer dbInitializer)
    {
        _dbInitializer = dbInitializer
            ?? throw new ArgumentNullException(nameof(dbInitializer));

        // Inicializa o banco antes de abrir as telas.
        InicializarBanco();

        InitializeComponent();

        // Aplica o tema salvo nas preferências.
        AplicarTema();

        // Recebe alterações de tema.
        WeakReferenceMessenger.Default.Register
            <TemaPreferencesUpdatedMessage>(
                this,
                (r, m) =>
                {
                    AplicarTema();
                });
    }

    private void InicializarBanco()
    {
        try
        {
            Task.Run(async () =>
            {
                await _dbInitializer.InicializarAsync();
            }).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Não foi possível inicializar o banco de dados SQLite. " +
                "Verifique a conexão e a estrutura do banco.",
                ex);
        }
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void AplicarTema()
    {
        UserAppTheme = Preferences.Get("Tema", "system") switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
