
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarCaminhoBanco();
    }

    private void CarregarTema()
    {
        var picker = this.FindByName<Picker>("TemaPicker");

        if (picker is null)
            return;

        picker.SelectedIndex = Preferences.Get("Tema", "system") switch
        {
            "light" => 0,
            "dark" => 1,
            _ => 2
        };
    }

    private void CarregarCaminhoBanco()
    {
        var entrada = this.FindByName<Entry>("SqliteCaminhoEntry");

        if (entrada is null)
            return;

        string caminhoPadrao = Path.Combine(
            FileSystem.AppDataDirectory,
            "db_academia_do_ze.db");

        entrada.Text = Preferences.Get("Sqlite_Caminho", caminhoPadrao);
    }

    private async void OnSalvarTemaClicked(object? sender, EventArgs e)
    {
        var picker = this.FindByName<Picker>("TemaPicker");

        if (picker is null)
            return;

        string tema = picker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Set("Tema", tema);

        WeakReferenceMessenger.Default.Send(
            new TemaPreferencesUpdatedMessage("TemaAlterado"));

        await DisplayAlertAsync("Sucesso", "Tema salvo com sucesso!", "OK");
    }

    private async void OnSalvarBancoClicked(object? sender, EventArgs e)
    {
        var entrada = this.FindByName<Entry>("SqliteCaminhoEntry");

        if (entrada is null)
            return;

        string caminho = entrada.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(caminho) ||
            !caminho.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
        {
            await DisplayAlertAsync(
                "Atenção",
                "Informe um caminho válido para um arquivo .db.",
                "OK");

            return;
        }

        Preferences.Set("Sqlite_Caminho", caminho);

        WeakReferenceMessenger.Default.Send(
            new BancoPreferencesUpdatedMessage("BancoAlterado"));

        await DisplayAlertAsync(
            "Sucesso",
            "Caminho do banco SQLite salvo!",
            "OK");
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }
}
