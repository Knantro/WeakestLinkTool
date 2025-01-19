using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление интро игры
/// </summary>
public class IntroPanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private int currentTitleIndex = 0;
    private string currentTitle;
    private bool isIntroFinished;
    private bool isIntroStarted;
    private bool isStartEnabled;

    /// <summary>
    /// Текст интро
    /// </summary>
    public string IntroText => $"Вот {WeakestLinkLogic.CurrentSession.AllPlayers.Count} участников сегодняшней игры.{Environment.NewLine}" +
        $"Из них только один сможет забрать денежный приз, размером до {WeakestLinkLogic.MaxPossibleGain.Decline("рубль", "рубля", "рублей")}{Environment.NewLine}" +
        $"Остальные покинут студию ни с чем, по мере того как раунд за раундом команда назовёт их \"слабым звеном\"";

    /// <summary>
    /// Страницы вступления
    /// </summary>
    public List<string> Titles { get; set; } = [
        "В эфире игра \"Слабое звено\"!",
        $"Каждый из {WeakestLinkLogic.CurrentSession.AllPlayers.Count} участников, находящихся сегодня в студии, может заработать до {WeakestLinkLogic.MaxPossibleGain.Decline("рубль", "рубля", "рублей")}",
        "Они не знают друг друга, но если хотят выиграть большой приз, они должны стать командой",
        $"{WeakestLinkLogic.CurrentSession.AllPlayers.Count - 1} из них уйдут отсюда ни с чем, ведь раунд за раундом мы будем терять игроков",
        "Тех, кого команда назовёт \"слабым звеном\". Вот эта команда",
    ];

    /// <summary>
    /// Было ли совершено первое пролистывание вступления
    /// </summary>
    public bool IsFirstNextTitleDone { get; set; }

    /// <summary>
    /// Началось ли интро
    /// </summary>
    public bool IsIntroStarted {
        get => isIntroStarted;
        set => SetField(ref isIntroStarted, value);
    }

    /// <summary>
    /// Возможно ли запустить интро
    /// </summary>
    public bool IsStartEnabled {
        get => isStartEnabled;
        set => SetField(ref isStartEnabled, value);
    }

    /// <summary>
    /// Закончилось ли интро
    /// </summary>
    public bool IsIntroFinished {
        get => isIntroFinished;
        set => SetField(ref isIntroFinished, value);
    }

    /// <summary>
    /// Является ли текущая страница вступления первой
    /// </summary>
    public bool IsFirstTitle => currentTitleIndex == 0;

    /// <summary>
    /// Является ли текущая страница вступления последней
    /// </summary>
    public bool IsLastTitle => currentTitleIndex == Titles.Count - 1;

    /// <summary>
    /// Текущая страница вступления
    /// </summary>
    public string CurrentTitle {
        get => currentTitle;
        set {
            SetField(ref currentTitle, value);
            OnPropertyChanged(nameof(IsFirstTitle));
            OnPropertyChanged(nameof(IsLastTitle));
        }
    }

    public RelayCommand NextTitleCommand => new(_ => ChangeTitle(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand BackTitleCommand => new(_ => ChangeTitle(false), _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Переходит к правилам игры
    /// </summary>
    public RelayCommand RulesCommand => new(_ => {
        logger.Info("Go to the rules");
        ChangeMWPage<RulesPage>();
        ChangePWPage<GameRulesPage>();
    }, _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Начинает вступление
    /// </summary>
    public RelayCommand StartIntroCommand => new(async _ => {
        logger.Info("Starting intro");
        IsIntroStarted = true;
        GetPlayerPageDataContext<IntroVM>().PlayIntro();
        await SoundManager.FadeWith(SoundName.INTRO, SoundName.OPENING_TITLES, SoundConst.OPENING_TITLES_FADE, TimeSpan.FromMilliseconds(SoundConst.OPENING_TITLES_POSITION_MS));
        CurrentTitle = Titles[currentTitleIndex];
        await Task.Delay(SoundConst.OPENING_TITLES_AWAIT_TO_START_GAME);
        IsIntroFinished = true;
    }, _ => IsStartEnabled && !IsIntroStarted && !mainWindowViewModel.IsMessageBoxVisible);

    public IntroPanelVM() {
        logger.SignedDebug();
        SoundManager.LoopPlay(SoundName.INTRO, SoundConst.INTRO_LOOP_POSITION_A, SoundConst.INTRO_LOOP_POSITION_B);
        WaitStartEnable();
    }

    /// <summary>
    /// Ждёт, когда можно запустить интро
    /// </summary>
    private async Task WaitStartEnable() {
        await Task.Delay(SoundConst.INTRO_START_COOLDOWN);
        IsStartEnabled = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Поменять страницу вступления
    /// </summary>
    /// <param name="forward">Направление изменения страницы. По умолчанию - True, то есть следующая</param>
    private void ChangeTitle(bool forward = true) {
        logger.Debug($"Changing title {(forward ? "forward" : "backward")}");
        if (forward && !IsFirstNextTitleDone) {
            SoundManager.Play(SoundName.SHORT_STING);
            SoundManager.SetVolumeCoefficient(SoundName.GENERAL_BED, SoundConst.GENERAL_BED_DEFAULT_VOLUME_COEFFICIENT);
            SoundManager.FadeWith(SoundName.SHORT_STING, SoundName.GENERAL_BED, null, SoundConst.GENERAL_BED_FADE_IN_START_GAME,
                soundInPositionA: SoundConst.GENERAL_BED_LOOP_POSITION_A, soundInPositionB: SoundConst.GENERAL_BED_LOOP_POSITION_B);
            IsFirstNextTitleDone = true;
        }

        CurrentTitle = Titles[forward ? ++currentTitleIndex : --currentTitleIndex];
    }
}