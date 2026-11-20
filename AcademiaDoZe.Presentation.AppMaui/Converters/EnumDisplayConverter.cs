
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public class EnumDisplayConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        string texto = value.ToString() ?? string.Empty;

        return string.Concat(
            texto.Select((caractere, indice) =>
                indice > 0 && char.IsUpper(caractere)
                    ? " " + caractere
                    : caractere.ToString()));
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return value ?? string.Empty;
    }
}
