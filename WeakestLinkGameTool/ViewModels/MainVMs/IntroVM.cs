using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class IntroVM : ViewModelBase {
    
    private int currentTitleIndex = 0;
    private string currentTitle;
    private bool isIntroFinished;

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
    public RelayCommand RulesCommand => new(_ => ChangeMWPage<RulesPage>());
    
    /// <summary>
    /// Начинает вступление
    /// </summary>
    public RelayCommand StartIntroCommand => new(_ => { // TODO: Исправить после введения музыки
        IsIntroFinished = true;
        CurrentTitle = Titles[currentTitleIndex];
    }); 
    
    public IntroVM() {
        
    }

    /// <summary>
    /// Поменять страницу вступления
    /// </summary>
    /// <param name="forward">Направление изменения страницы. По умолчанию - True, то есть следующая</param>
    private void ChangeTitle(bool forward = true) {
        if (forward && !IsFirstNextTitleDone) {
            // TODO: Музыка
            IsFirstNextTitleDone = true;
        }
        
        CurrentTitle = Titles[forward ? ++currentTitleIndex : --currentTitleIndex];
    }
}