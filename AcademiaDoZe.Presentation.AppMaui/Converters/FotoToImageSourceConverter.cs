
using AcademiaDoZe.Application.DTOs;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.IO;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public class FotoToImageSourceConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is ArquivoDto foto &&
            foto.Conteudo != null &&
            foto.Conteudo.Length > 0)
        {
            byte[] conteudo = foto.Conteudo;

            return ImageSource.FromStream(
                () => new MemoryStream(conteudo));
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
