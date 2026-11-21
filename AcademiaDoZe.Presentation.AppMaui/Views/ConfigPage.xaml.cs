
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    private const string ChaveTema = "Tema";
    private const string ChaveCaminhoSqlite = "Sqlite_Caminho";
    private const string ChaveCaminhoSqliteLegado = "SqliteCaminho";

    public ConfigPage()
    {
        InitializeComponent();

        CarregarTema();
        CarregarBanco();
    }

    #region Tema

    private void CarregarTema()
    {
        string tema = Preferences.Get(ChaveTema, "system");

        TemaPicker.SelectedIndex = tema switch
        {
            "light" => 0,
            "dark" => 1,
            _ => 2
        };
    }

    private async void OnSalvarTemaClicked(object? sender, EventArgs e)
    {
        string tema = TemaPicker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Set(ChaveTema, tema);

        WeakReferenceMessenger.Default.Send(
            new TemaPreferencesUpdatedMessage("TemaAlterado"));

        await DisplayAlert(
            "Sucesso",
            "Tema salvo com sucesso!",
            "OK");

        await Shell.Current.GoToAsync("//dashboard");
    }

    #endregion

    #region Banco de Dados SQLite

    private void CarregarBanco()
    {
        string caminhoPadrao = Path.Combine(
            FileSystem.AppDataDirectory,
            "db_academia_do_ze.db");

        string caminhoSalvo = Preferences.Get(
            ChaveCaminhoSqlite,
            Preferences.Get(
                ChaveCaminhoSqliteLegado,
                caminhoPadrao));

        SqliteCaminhoEntry.Text =
            string.IsNullOrWhiteSpace(caminhoSalvo)
                ? caminhoPadrao
                : caminhoSalvo;
    }

    private async void OnSalvarBdClicked(object? sender, EventArgs e)
    {
        string? caminhoInformado = SqliteCaminhoEntry.Text;

        if (string.IsNullOrWhiteSpace(caminhoInformado))
        {
            await DisplayAlert(
                "Validação",
                "Informe o caminho do arquivo do banco SQLite.",
                "OK");

            return;
        }

        string caminho = caminhoInformado.Trim();

        try
        {
            string? diretorio = Path.GetDirectoryName(caminho);

            if (string.IsNullOrWhiteSpace(Path.GetFileName(caminho)) ||
                !string.Equals(
                    Path.GetExtension(caminho),
                    ".db",
                    StringComparison.OrdinalIgnoreCase))
            {
                await DisplayAlert(
                    "Validação",
                    "Informe um caminho válido para um arquivo com extensão .db.",
                    "OK");

                return;
            }

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            Preferences.Set(ChaveCaminhoSqlite, caminho);
            Preferences.Set(ChaveCaminhoSqliteLegado, caminho);

            WeakReferenceMessenger.Default.Send(
                new BancoPreferencesUpdatedMessage("BancoAlterado"));

            await DisplayAlert(
                "Sucesso",
                "Caminho do banco SQLite salvo com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("//dashboard");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro",
                $"Não foi possível salvar o caminho do banco de dados.\n{ex.Message}",
                "OK");
        }
    }

    #endregion

    #region Cancelar

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }

    #endregion

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
