using System.Windows.Controls;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class FinalRoundPage : UserControl {
    public FinalRoundPage() {
        InitializeComponent();

        (DataContext as FinalRoundVM)!.FinalRoundPanelVisibilityChanged += (_, show) => {
            if (show) ShowFinalRoundPanel();
            else HideFinalRoundPanel();
        };
        
        (DataContext as FinalRoundVM)!.TotalBankVisibilityChanged += (_, show) => {
            if (show) ShowTotalBank();
            else HideTotalBank();
        };
    }

    private void ShowFinalRoundPanel() {
        if (FinalRoundPanel.Opacity < 10e-9) {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            FinalRoundPanel.BeginAnimation(OpacityProperty, anim);
        }
    }

    private void HideFinalRoundPanel() {
        if (Math.Abs(FinalRoundPanel.Opacity - 1) < 10e-9) {
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            FinalRoundPanel.BeginAnimation(OpacityProperty, anim);
        }
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