using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WeakestLinkGameTool.ViewModels.PlayerVMs;

namespace WeakestLinkGameTool.Views.PlayerPages;

public partial class EndGamePage : UserControl {
    public EndGamePage() {
        InitializeComponent();
        
        Loaded += (_, _) => {
            StartCredits();
        };

        (DataContext as EndGameVM)!.CompleteCreditsRaised += (_, _) => {
            CompleteCreditsImmediately();
        };
    }

    private void CompleteCreditsImmediately() {
        // anim.Duration = TimeSpan.FromSeconds(0);

        ScrollViewerContent.Opacity = 0;
        ThanksForGame.Opacity = 1;
    }

    private void StartCredits() {
        var anim = new DoubleAnimation(1100, -ScrollViewerContent.ExtentHeight, TimeSpan.FromSeconds(30));
        anim.Completed += (_, _) => {
            ThanksForGame.Opacity = 1;
        };
        
        if (ScrollContent.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            ScrollContent.RenderTransform = transform;
        }
        
        transform.BeginAnimation(TranslateTransform.YProperty, anim);
    }
}