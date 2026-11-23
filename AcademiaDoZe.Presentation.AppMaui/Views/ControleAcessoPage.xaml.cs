using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ControleAcessoPage : ContentPage
{
    public ControleAcessoPage(ControleAcessoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}