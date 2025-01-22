using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана объявления слабого звена
/// </summary>
public class WalkAShamePanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private bool isWeakestLinkDeclared;
    private bool preRoundInstructionAvailable;

    /// <summary>
    /// Игрок, объявленный слабым звеном
    /// </summary>
    public Player KickedPlayer { get; set; }

    /// <summary>
    /// Объявлено ли слабое звено
    /// </summary>
    public bool IsWeakestLinkDeclared {
        get => isWeakestLinkDeclared;
        set => SetField(ref isWeakestLinkDeclared, value);
    }

    /// <summary>
    /// Доступна ли инструкция перед раундом
    /// </summary>
    public bool PreRoundInstructionAvailable {
        get => preRoundInstructionAvailable;
        set => SetField(ref preRoundInstructionAvailable, value);
    }

    public RelayCommand DeclareWeakestLinkCommand => new(async _ => await DeclareWeakestLink(), _ => !IsWeakestLinkDeclared &&!mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand PreRoundInstructionCommand => new(async _ => await PreRoundInstruction(), _ => PreRoundInstructionAvailable && !mainWindowViewModel.IsMessageBoxVisible);

    public WalkAShamePanelVM() {
        logger.SignedDebug();
        KickedPlayer = WeakestLinkLogic.GetCurrentKickedPlayer();
        EnterCommand = DeclareWeakestLinkCommand;
    }

    /// <summary>
    /// Объявляет слабое звено
    /// </summary>
    private async Task DeclareWeakestLink() {
        logger.Info($"Declare {KickedPlayer.Name} as a weakest link");
        IsWeakestLinkDeclared = true;
        SoundManager.LoopPlay(SoundName.WALK_OF_SHAME, SoundConst.WALK_OF_SHAME_LOOP_POSITION_A, SoundConst.WALK_OF_SHAME_LOOP_POSITION_B);
        await Task.Delay(SoundConst.GENERAL_BED_WALK_OF_SHAME_DELAY);
        SoundManager.Pause(SoundName.GENERAL_BED);
        await Task.Delay(SoundConst.GENERAL_BED_WALK_OF_SHAME_DELAY);
        ChangePWPage<WalkAShamePage>();
        PreRoundInstructionAvailable = true;
        EnterCommand = PreRoundInstructionCommand;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Переходит на экран с инструкцией перед раундом
    /// </summary>
    private async Task PreRoundInstruction() {
        logger.Debug("Go to the pre-round instruction");
        PreRoundInstructionAvailable = false;
        CommandManager.InvalidateRequerySuggested();
        SoundManager.FadeWith(SoundName.WALK_OF_SHAME, SoundName.AFTER_INTERVIEW_STING, fadeOutMilliseconds: SoundConst.WALK_OF_SHAME_AFTER_INTERVIEW_STING_FADE_OUT);
        await Task.Delay(SoundConst.GENERAL_BED_AFTER_INTERVIEW_STING_DELAY);
        SoundManager.Resume(SoundName.GENERAL_BED);
        ChangeMWPage<NextRoundInstructionPage>();
    }
}