using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeakestLinkGameTool.Converters;

/// <summary>
/// Конвертер преобразование <see cref="HorizontalAlignment"/> в <see cref="TextAlignment"/>
/// </summary>
public class TextAlignmentConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not HorizontalAlignment alignment) {
            return null;
        }

        return alignment switch {
            HorizontalAlignment.Left => TextAlignment.Left,
            HorizontalAlignment.Center => TextAlignment.Center,
            HorizontalAlignment.Right => TextAlignment.Right,
            HorizontalAlignment.Stretch => TextAlignment.Justify,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}