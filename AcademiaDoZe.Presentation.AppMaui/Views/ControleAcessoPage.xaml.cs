
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ControleAcessoPage : ContentPage
{
    public ControleAcessoPage()
    {
        InitializeComponent();

        var servicos =
            Microsoft.Maui.Controls.Application.Current?
                .Handler?
                .MauiContext?
                .Services;

        if (servicos == null)
        {
            throw new InvalidOperationException(
                "Não foi possível acessar os serviços da aplicação.");
        }

        BindingContext =
            servicos.GetRequiredService<ControleAcessoViewModel>();
    }
}
