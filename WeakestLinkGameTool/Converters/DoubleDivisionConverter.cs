using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeakestLinkGameTool.Converters;

public class DoubleDivisionConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not double val || !double.TryParse(parameter?.ToString(), out var param)) {
            return null;
        }

        return val / param;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}