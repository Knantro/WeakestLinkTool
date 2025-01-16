using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class IntroPanelVM : ViewModelBase {
    
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
    /// 
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

    public RelayCommand NextTitleCommand => new(_ => ChangeTitle());
    public RelayCommand BackTitleCommand => new(_ => ChangeTitle(false));
    
    /// <summary>
    /// Переходит к правилам игры
    /// </summary>
    public RelayCommand RulesCommand => new(_ => {
        ChangeMWPage<RulesPage>();
        ChangePWPage<GameRulesPage>();
    });
    
    /// <summary>
    /// Начинает вступление
    /// </summary>
    public RelayCommand StartIntroCommand => new(async _ => {
        IsIntroStarted = true;
        GetPlayerPageDataContext<IntroVM>().PlayIntro();
        await SoundManager.FadeWith(SoundName.INTRO, SoundName.OPENING_TITLES, SoundConst.OPENING_TITLES_FADE, TimeSpan.FromMilliseconds(SoundConst.OPENING_TITLES_POSITION_MS));
        CurrentTitle = Titles[currentTitleIndex];
        await Task.Delay(11 * 1000); // TODO: Magic const
        IsIntroFinished = true;
    }, _ => IsStartEnabled && !IsIntroStarted); 
    
    public IntroPanelVM() {
        SoundManager.LoopPlay(SoundName.INTRO, SoundConst.INTRO_LOOP_POSITION_A, SoundConst.INTRO_LOOP_POSITION_B);
        WaitStartEnable();
    }

    private async Task WaitStartEnable() {
        await Task.Delay(3000); // TODO: Magic const
        IsStartEnabled = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Поменять страницу вступления
    /// </summary>
    /// <param name="forward">Направление изменения страницы. По умолчанию - True, то есть следующая</param>
    private void ChangeTitle(bool forward = true) {
        if (forward && !IsFirstNextTitleDone) {
            SoundManager.Play(SoundName.SHORT_STING);
            SoundManager.SetVolumeCoefficient(SoundName.GENERAL_BED, 0.5f);
            SoundManager.FadeWith(SoundName.SHORT_STING, SoundName.GENERAL_BED, null, 3000, // TODO: Magic const
                soundInPositionA: SoundConst.GENERAL_BED_LOOP_POSITION_A, soundInPositionB: SoundConst.GENERAL_BED_LOOP_POSITION_B);
            IsFirstNextTitleDone = true;
        }
        
        CurrentTitle = Titles[forward ? ++currentTitleIndex : --currentTitleIndex];
    }
}