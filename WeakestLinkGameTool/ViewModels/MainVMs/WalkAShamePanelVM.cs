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

    public RelayCommand DeclareWeakestLinkCommand => new(_ => DeclareWeakestLink());
    public RelayCommand PreRoundInstructionCommand => new(_ => PreRoundInstruction());

    public WalkAShamePanelVM() {
        KickedPlayer = WeakestLinkLogic.GetCurrentKickedPlayer();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void DeclareWeakestLink() {
        Task.Run(async () => {
            SoundManager.LoopPlay(SoundName.WALK_OF_SHAME, SoundConst.WALK_OF_SHAME_LOOP_POSITION_A, SoundConst.WALK_OF_SHAME_LOOP_POSITION_B);
            await Task.Delay(1000); // TODO: Magic const
            SoundManager.Pause(SoundName.GENERAL_BED);
        });
        IsWeakestLinkDeclared = true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void PreRoundInstruction() {
        Task.Run(async () => {
            await SoundManager.FadeWith(SoundName.WALK_OF_SHAME, SoundName.AFTER_INTERVIEW_STING, fadeOutMilliseconds: 3000); // TODO: Magic const
            await Task.Delay(2000); // TODO: Magic const
            SoundManager.Resume(SoundName.GENERAL_BED);
        });
        ChangeMWPage<NextRoundInstructionPage>();
    }
}