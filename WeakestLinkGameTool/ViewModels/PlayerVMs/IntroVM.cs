using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class IntroVM : ViewModelBase {
    
    public event EventHandler IntroPlayRequested;
    
    private bool isIntroVisible;

    /// <summary>
    /// 
    /// </summary>
    public bool IsIntroVisible {
        get => isIntroVisible;
        set => SetField(ref isIntroVisible, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlayIntro() {
        IntroPlayRequested?.Invoke(this, EventArgs.Empty);
    }
}