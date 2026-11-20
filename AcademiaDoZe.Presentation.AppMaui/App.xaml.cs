using Microsoft.Maui;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MauiApplication = Microsoft.Maui.Controls.Application;
using AppTheme = Microsoft.Maui.ApplicationModel.AppTheme;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : MauiApplication
{
    public App()
    {
        InitializeComponent();

        AplicarTema();

        WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(
            this,
            (r, m) =>
            {
                AplicarTema();
            });
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void AplicarTema()
    {
        string tema = Preferences.Get("Tema", "system");

        switch (tema)
        {
            case "light":
                UserAppTheme = AppTheme.Light;
                break;

            case "dark":
                UserAppTheme = AppTheme.Dark;
                break;

            default:
                UserAppTheme = AppTheme.Unspecified;
                break;
        }
    }
}
