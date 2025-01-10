using System.Windows.Controls;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class GameRulesPage : UserControl {
    public GameRulesPage() {
        InitializeComponent();

        (DataContext as GameRulesVM).GameRulesVisibilityChanged += (_, show) => {
            if (show) ShowMoneyTree();
            else HideMoneyTree();
        };
    }
    
    private void ShowMoneyTree() {
        if (MoneyTree.Opacity < 10e-9) {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            MoneyTree.BeginAnimation(OpacityProperty, anim);
        }
    }

    private void HideMoneyTree() {
        if (Math.Abs(MoneyTree.Opacity - 1) < 10e-9) {
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            MoneyTree.BeginAnimation(OpacityProperty, anim);
        }
    }
}