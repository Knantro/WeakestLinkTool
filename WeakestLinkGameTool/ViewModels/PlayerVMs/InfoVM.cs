using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class InfoVM : ViewModelBase {

    private Brush background;

    /// <summary>
    /// 
    /// </summary>
    public Brush Background {
        get => background;
        set => SetField(ref background, value);
    }
    
    public InfoVM() {
        Background = Application.Current.Resources["CommonBackgroundBrush"] as LinearGradientBrush;
    }
}