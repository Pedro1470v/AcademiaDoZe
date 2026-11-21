
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AcademiaDoZe.Application.Enums;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);

        FieldInfo? field = value.GetType().GetField(value.ToString());

        if (field is null)
            return value.ToString();

        DisplayAttribute? attribute =
            field.GetCustomAttribute<DisplayAttribute>();

        return attribute?.GetName() ?? value.ToString();
    }
}
