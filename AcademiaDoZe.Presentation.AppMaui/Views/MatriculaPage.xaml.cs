
using System;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaPage : ContentPage
{
    public MatriculaPage(MatriculaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MatriculaViewModel viewModel)
        {
            await viewModel.InitializeAsync();
            CalculateFimFromPlanoAndInicio();
        }
    }

    private void PlanoPicker_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        CalculateFimFromPlanoAndInicio();
    }

    private void InicioDatePicker_DateSelected(
        object? sender,
        DateChangedEventArgs e)
    {
        CalculateFimFromPlanoAndInicio();
    }

    private void CalculateFimFromPlanoAndInicio()
    {
        if (BindingContext is not MatriculaViewModel vm)
            return;

        var plano = vm.Matricula.Plano;
        var inicio = vm.Matricula.DataInicio;

        DateOnly fim = plano switch
        {
            MatriculaPlano.Mensal => inicio.AddMonths(1),
            MatriculaPlano.Trimestral => inicio.AddMonths(3),
            MatriculaPlano.Semestral => inicio.AddMonths(6),
            MatriculaPlano.Anual => inicio.AddYears(1),
            _ => inicio
        };

        vm.Matricula.DataFim = fim;

        if (FimDatePicker != null)
        {
            FimDatePicker.Date = fim.ToDateTime(
                new TimeOnly(0, 0));
        }
    }
}
