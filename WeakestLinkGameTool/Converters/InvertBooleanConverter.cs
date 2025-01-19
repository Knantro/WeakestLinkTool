using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeakestLinkGameTool.Converters;

/// <summary>
/// Конвертер инверсии логического значения биндинга 
/// </summary>
public class InvertBooleanConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not bool invert) {
            return DependencyProperty.UnsetValue;
        }

        return !invert;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}