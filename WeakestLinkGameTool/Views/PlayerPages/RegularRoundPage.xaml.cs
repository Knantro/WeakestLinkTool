using System.Windows.Controls;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class RegularRoundPage : UserControl {
    public RegularRoundPage() {
        InitializeComponent();

        (DataContext as RegularRoundVM).RoundPanelVisibilityChanged += (_, show) => {
            if (show) ShowRoundPanel();
            else HideRoundPanel();
        };
    }
    
    private void ShowRoundPanel() {
        if (RoundPanelGrid.Opacity < 10e-9) {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            RoundPanelGrid.BeginAnimation(OpacityProperty, anim);
        }
    }

    private void HideRoundPanel() {
        if (Math.Abs(RoundPanelGrid.Opacity - 1) < 10e-9) {
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            RoundPanelGrid.BeginAnimation(OpacityProperty, anim);
        }
    }
}