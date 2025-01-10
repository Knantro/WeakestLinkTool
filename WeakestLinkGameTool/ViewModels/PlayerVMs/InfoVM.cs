using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class InfoVM : ViewModelBase {
    private int currentTotalBank;
    
    public event EventHandler<bool> TotalBankVisibilityChanged;

    /// <summary>
    /// 
    /// </summary>
    public int CurrentTotalBank {
        get => currentTotalBank;
        set => SetField(ref currentTotalBank, value);
    }
    
    public InfoVM() {
        CurrentTotalBank = WeakestLinkLogic.CurrentSession.FullBank;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleFullBankVisibility(bool isShow) {
        TotalBankVisibilityChanged?.Invoke(this, isShow);
    }
}