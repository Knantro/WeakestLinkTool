using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class IntroPage : UserControl {
    public IntroPage() {
        InitializeComponent();

        WLIntro.MediaFailed += (_, e) => {
            Debugger.Break();
        };
        
        var vm = DataContext as IntroVM;
        vm!.IntroPlayRequested += (_, _) => {
            vm.IsIntroVisible = true;
            WLIntro.Play();
        };
    }
}