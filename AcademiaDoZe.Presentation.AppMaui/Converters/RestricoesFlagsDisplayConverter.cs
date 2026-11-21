
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using AcademiaDoZe.Domain.Enums;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public sealed class RestricoesFlagsDisplayConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not MatriculaRestricoes flags)
            return string.Empty;

        if (flags == 0)
            return "Nenhuma";

        var partes = new List<string>();

        foreach (MatriculaRestricoes restricao
                 in Enum.GetValues(typeof(MatriculaRestricoes)))
        {
            if (restricao == 0)
                continue;

            if ((flags & restricao) == restricao)
            {
                var membro = typeof(MatriculaRestricoes)
                    .GetMember(restricao.ToString())
                    .FirstOrDefault();

                var display = membro?
                    .GetCustomAttribute<DisplayAttribute>();

                partes.Add(display?.Name ?? restricao.ToString());
            }
        }

        return partes.Count > 0
            ? string.Join(", ", partes)
            : "Nenhuma";
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
