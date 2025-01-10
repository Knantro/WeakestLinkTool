using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class RulesVM : ViewModelBase {
    
    private int currentRuleIndex = 0;
    private string currentRule;

    /// <summary>
    /// Страницы вступления
    /// </summary>
    public List<string> Rules { get; set; } = [
        "Теперь правила нашей игры",
        $"В каждом раунде вы можете выиграть до {WeakestLinkLogic.MoneyTree.Max(x => x.Value).Decline("рубль", "рубля", "рублей")} за ограниченный период времени",
        "Самый быстрый способ это выстроить цепь из 8 правильных ответов и отправить в банк заветную сумму",
        "Если вы ответите неверно, вы разорвёте цепь правильных ответов и потеряете деньги, заработанные командой в этой цепочке",
        "Но если вы успеете сказать слово \"банк\", после того, как услышите своё имя, но до того, как будет задан вопрос - вы сохраните деньги, однако цепь ответов начнёте строить заново",
        "Помните, что в каждом раунде вашим выигрышем становятся только те деньги, которые вы успели положить в банк. Эта сумма переходит с вами в следующий раунд",
        "Сумма заработанных в каждом раунде денег и составит конечный выигрыш",
        $"На первый раунд у вас будет ровно 3 минуты и мы начнём с игрока, чьё имя первое по алфавиту, это вы - {WeakestLinkLogic.CurrentSession.AllPlayers.OrderBy(x => x.Name).First().Name}",
        "Итак, играем в \"Слабое звено\"",
    ];

    /// <summary>
    /// Является ли текущая страница вступления первой
    /// </summary>
    public bool IsFirstRule => currentRuleIndex == 0;
    
    /// <summary>
    /// Является ли текущая страница вступления последней
    /// </summary>
    public bool IsLastRule => currentRuleIndex == Rules.Count - 1;
    
    /// <summary>
    /// Текущая страница вступления
    /// </summary>
    public string CurrentRule {
        get => currentRule;
        set {
            SetField(ref currentRule, value);
            OnPropertyChanged(nameof(IsFirstRule));
            OnPropertyChanged(nameof(IsLastRule));
        }
    }

    public RelayCommand NextRuleCommand => new(_ => ChangeRule());
    public RelayCommand BackRuleCommand => new(_ => ChangeRule(false));
    public RelayCommand StartDemoCommand => new(async _ => await GetPlayerPageDataContext<GameRulesVM>().StartDemo());
    public RelayCommand StopDemoCommand => new(_ => GetPlayerPageDataContext<GameRulesVM>().StopDemo());
    public RelayCommand ShowRoundPanelCommand => new(_ => GetPlayerPageDataContext<GameRulesVM>().ShowRoundPanel());
    public RelayCommand HideRoundPanelCommand => new(_ => GetPlayerPageDataContext<GameRulesVM>().HideRoundPanel());
    public RelayCommand WrongAnswerDemoCommand => new(_ => GetPlayerPageDataContext<GameRulesVM>().WrongAnswerDemo());
    public RelayCommand BankDemoCommand => new(_ => GetPlayerPageDataContext<GameRulesVM>().BankDemo());
    
    /// <summary>
    /// Переходит к правилам игры
    /// </summary>
    public RelayCommand StartGameCommand => new(_ => ChangeMWPage<RegularRoundPanelPage>());
    
    public RulesVM() {
        CurrentRule = Rules[currentRuleIndex]; 
        SoundManager.Play(SoundName.GENERAL_STING);
    }

    /// <summary>
    /// Поменять страницу правил
    /// </summary>
    /// <param name="forward">Направление изменения страницы. По умолчанию - True, то есть следующая</param>
    private void ChangeRule(bool forward = true) {
        CurrentRule = Rules[forward ? ++currentRuleIndex : --currentRuleIndex];
    }
}