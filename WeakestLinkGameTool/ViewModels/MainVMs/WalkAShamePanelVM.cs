using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class WalkAShamePanelVM : ViewModelBase {
    private bool isWeakestLinkDeclared;
    private bool preRoundInstructionAvailable;

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
    
    /// <summary>
    /// 
    /// </summary>
    public bool PreRoundInstructionAvailable {
        get => preRoundInstructionAvailable;
        set => SetField(ref preRoundInstructionAvailable, value);
    }

    public RelayCommand DeclareWeakestLinkCommand => new(async _ => await DeclareWeakestLink());
    public RelayCommand PreRoundInstructionCommand => new(async _ => await PreRoundInstruction(), _ => PreRoundInstructionAvailable);

    public WalkAShamePanelVM() {
        KickedPlayer = WeakestLinkLogic.GetCurrentKickedPlayer();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private async Task DeclareWeakestLink() {
        IsWeakestLinkDeclared = true;
        SoundManager.LoopPlay(SoundName.WALK_OF_SHAME, SoundConst.WALK_OF_SHAME_LOOP_POSITION_A, SoundConst.WALK_OF_SHAME_LOOP_POSITION_B);
        await Task.Delay(1000); // TODO: Magic const
        SoundManager.Pause(SoundName.GENERAL_BED);
        await Task.Delay(1000);
        ChangePWPage<WalkAShamePage>();
        PreRoundInstructionAvailable = true;
        CommandManager.InvalidateRequerySuggested();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private async Task PreRoundInstruction() {
        PreRoundInstructionAvailable = false;
        CommandManager.InvalidateRequerySuggested();
        SoundManager.FadeWith(SoundName.WALK_OF_SHAME, SoundName.AFTER_INTERVIEW_STING, fadeOutMilliseconds: 3000); // TODO: Magic const
        await Task.Delay(2000); // TODO: Magic const
        SoundManager.Resume(SoundName.GENERAL_BED);
        ChangeMWPage<NextRoundInstructionPage>();
    }
}