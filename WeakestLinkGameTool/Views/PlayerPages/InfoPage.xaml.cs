using System.Windows.Controls;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class InfoPage : UserControl {
    public InfoPage() {
        InitializeComponent();

        (DataContext as InfoVM).TotalBankVisibilityChanged += (_, show) => {
            if (show) ShowTotalBank();
            else HideTotalBank();
        };
    }

    private void ShowTotalBank() {
        if (TotalBankGrid.Opacity < 10e-9) {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            TotalBankGrid.BeginAnimation(OpacityProperty, anim);
        }
    }

    private void HideTotalBank() {
        if (Math.Abs(TotalBankGrid.Opacity - 1) < 10e-9) {
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            TotalBankGrid.BeginAnimation(OpacityProperty, anim);
        }
    }
}