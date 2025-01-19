using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока с информационной панелью
/// </summary>
public class InfoVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private int currentTotalBank;

    /// <summary>
    /// Событие изменения видимости текущего банка игры
    /// </summary>
    public event EventHandler<bool> TotalBankVisibilityChanged;

    /// <summary>
    /// Текущий банк игры
    /// </summary>
    public int CurrentTotalBank {
        get => currentTotalBank;
        set => SetField(ref currentTotalBank, value);
    }

    public InfoVM() {
        logger.SignedDebug();
        CurrentTotalBank = WeakestLinkLogic.CurrentSession.FullBank;
    }

    /// <summary>
    /// Меняет видимость панели банка игры
    /// </summary>
    public void ToggleFullBankVisibility(bool isShow) {
        logger.SignedDebug($"isShow = {isShow}");
        TotalBankVisibilityChanged?.Invoke(this, isShow);
    }
}