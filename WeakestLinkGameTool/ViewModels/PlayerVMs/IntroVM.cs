using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class IntroVM : ViewModelBase {
    
    private bool isIntroVisible;
    
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler IntroPlayRequested;

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