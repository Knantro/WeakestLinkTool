using System.Windows.Controls;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class IntroPage : UserControl {
    public IntroPage() {
        InitializeComponent();
        
        var vm = DataContext as IntroVM;
        vm!.IntroPlayRequested += (_, _) => {
            vm.IsIntroVisible = true;
            WLIntro.Play();
        };
    }
}