// Pedro Vinicius

using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public class ByteArrayToImageSourceConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not byte[] conteudo || conteudo.Length == 0)
            return null;

        return ImageSource.FromStream(
            () => new MemoryStream(conteudo));
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}