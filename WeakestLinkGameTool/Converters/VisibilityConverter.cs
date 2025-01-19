using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeakestLinkGameTool.Converters;

/// <summary>
/// Конвертер преобразования логического значения в значение типа <see cref="Visibility"/>
/// </summary>
public class VisibilityConverter : IValueConverter {
    
    public bool Invert { get; set; }
    
    public bool Collapse { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not bool isVisible) {
            return DependencyProperty.UnsetValue;
        }

        isVisible = Invert ? !isVisible : isVisible;

        return isVisible 
            ? Visibility.Visible 
            : Collapse ? Visibility.Collapsed : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}