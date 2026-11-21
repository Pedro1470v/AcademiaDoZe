
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class DashboardListPage : ContentPage
{
    public DashboardListPage()
    {
        InitializeComponent();
    }

    private async void OnLogradourosTapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//logradouros");
    }

    private async void OnAlunosTapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//alunos");
    }

    private async void OnMatriculasTapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//matriculas");
    }

    private async void OnColaboradoresTapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//colaboradores");
    }
}
