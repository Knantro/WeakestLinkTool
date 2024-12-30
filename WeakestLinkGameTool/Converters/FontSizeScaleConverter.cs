using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeakestLinkGameTool.Converters;

class FontSizeScaleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not double d || !double.TryParse(parameter?.ToString(), CultureInfo.InvariantCulture, out var param)) return null;
        return d * param;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}