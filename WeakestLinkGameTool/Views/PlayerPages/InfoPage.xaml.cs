using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class InfoPage : UserControl {
    public InfoPage() {
        InitializeComponent();
        
        
    }

    private void ShowTotalBank() {
        var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
        TotalBankGrid.BeginAnimation(OpacityProperty, anim);
    }

    private void HideTotalBank() {
        var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
        TotalBankGrid.BeginAnimation(OpacityProperty, anim);
    }
}