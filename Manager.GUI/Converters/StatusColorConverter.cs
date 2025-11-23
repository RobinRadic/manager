using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Manager.GUI.Converters;

public class StatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEnabled)
        {
            return isEnabled ? Brushes.LimeGreen : Brushes.Gray;
        }
        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}