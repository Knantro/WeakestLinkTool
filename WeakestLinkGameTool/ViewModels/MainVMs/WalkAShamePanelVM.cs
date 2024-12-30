using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class WalkAShamePanelVM : ViewModelBase {
    
    private bool isWeakestLinkDeclared;

    /// <summary>
    /// 
    /// </summary>
    public Player KickedPlayer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsWeakestLinkDeclared {
        get => isWeakestLinkDeclared;
        set => SetField(ref isWeakestLinkDeclared, value);
    }

    public RelayCommand DeclareWeakestLinkCommand => new(_ => IsWeakestLinkDeclared = true);
    public RelayCommand PreRoundInstructionCommand => new(_ => PreRoundInstruction());

    public WalkAShamePanelVM() {
        KickedPlayer = WeakestLinkLogic.GetCurrentKickedPlayer();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void PreRoundInstruction() {
        // TODO: Музыка
        ChangeMWPage<NextRoundInstructionPage>();
    }
}