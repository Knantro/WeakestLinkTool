using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока 
/// </summary>
public class IntroVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private bool isIntroVisible;
    
    /// <summary>
    /// Событие запроса проигрыша видео интро
    /// </summary>
    public event EventHandler IntroPlayRequested;

    /// <summary>
    /// Видно ли интро
    /// </summary>
    public bool IsIntroVisible {
        get => isIntroVisible;
        set => SetField(ref isIntroVisible, value);
    }

    /// <summary>
    /// Проигрывает видео интро
    /// </summary>
    public void PlayIntro() {
        logger.SignedDebug();
        IntroPlayRequested?.Invoke(this, EventArgs.Empty);
    }
}