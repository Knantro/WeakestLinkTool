using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class NextRoundInstructionVM : ViewModelBase {
    
    private int currentInstructionIndex = 0;
    private string currentInstruction;
    private bool showTotalBankToggle = true;

    public event EventHandler FullBankVisibleChanged;

    /// <summary>
    /// Страницы вступления
    /// </summary>
    public List<string> Instructions { get; set; } = [
        $"Раунд {WeakestLinkLogic.CurrentSession.CurrentRound.Number + 1}",
        rand.Next(2) == 0 
            ? $"И у вас в банке {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}" 
            : $"И к этому моменту вам удалось заработать {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}",
        WeakestLinkLogic.CurrentSession.FullBank == WeakestLinkLogic.MoneyTree.Max(x => x.Value) * WeakestLinkLogic.CurrentSession.CurrentRound.Number 
            ? rand.Next(2) == 0 ? "Пока что, максимально возможный банк на текущий период игры" : "Из максимально возможного банка игры"
            : rand.Next(2) == 0 
                ? $"Хотя могло быть {(WeakestLinkLogic.MoneyTree.Max(x => x.Value) * WeakestLinkLogic.CurrentSession.CurrentRound.Number).Decline("рубль", "рубля", "рублей")}"
                : rand.Next(2) == 0 
                    ? $"Если бы вы достигли своей цели, то эта сумма составляла бы {(WeakestLinkLogic.MoneyTree.Max(x => x.Value) * WeakestLinkLogic.CurrentSession.CurrentRound.Number).Decline("рубль", "рубля", "рублей")}" 
                    : $"Из возможных {(WeakestLinkLogic.MoneyTree.Max(x => x.Value) * WeakestLinkLogic.CurrentSession.CurrentRound.Number).Decline("рубль", "рубля", "рублей")}",
        $"Осталось только {WeakestLinkLogic.CurrentSession.ActivePlayers.Count.Decline("игрок", "игрока", "игроков")}",
        WeakestLinkLogic.CurrentSession.CurrentRound.Number + 1 == WeakestLinkLogic.CurrentSession.AllPlayers.Count - 1 
            ? $"{WeakestLinkLogic.CurrentSession.ActivePlayers[0].Name}, {WeakestLinkLogic.CurrentSession.ActivePlayers[1].Name}. На этот раз в вашем распоряжении будет всего 90 секунд (1,5 минуты)"
            : rand.Next(2) == 0 ? "Раунд будет короче (ещё) на 10 секунд" : "И мы отбираем у вас (ещё) 10 секунд",
        WeakestLinkLogic.CurrentSession.CurrentRound.Number + 1 == WeakestLinkLogic.CurrentSession.AllPlayers.Count - 1 
            ? $"Но любая сумма, которую вы заработаете в этом раунде будет удвоена и добавлена к той сумме, которая уже есть в банке. Это и составит финальный приз. В этом раунде на кону {(WeakestLinkLogic.MoneyTree.Max(x => x.Value) * 2).Decline("рубль", "рубля", "рублей")}" 
            : null,
        WeakestLinkLogic.CurrentSession.ActivePlayers.FirstOrDefault(x => x.IsStrongestLink) == null 
            ? $"Так как вы избавились от сильного звена прошлого раунда, то мы начнём с игрока, следующего по статистике, это {WeakestLinkLogic.CurrentSession.CurrentRound.Statistics.PlayersStatistics.Values.First(x => !x.Player.IsKicked).Player.Name}" 
            : $"И мы начнём с самого сильного звена прошлого раунда, это {WeakestLinkLogic.CurrentSession.ActivePlayers.First(x => x.IsStrongestLink).Name}",
        "Итак, играем в \"Слабое звено\"",
    ];

    /// <summary>
    /// Является ли текущая страница вступления первой
    /// </summary>
    public bool IsFirstInstruction => currentInstructionIndex == 0;
    
    /// <summary>
    /// Является ли текущая страница вступления последней
    /// </summary>
    public bool IsLastInstruction => currentInstructionIndex == Instructions.Count - 1;
    
    /// <summary>
    /// Текущая страница вступления
    /// </summary>
    public string CurrentInstruction {
        get => currentInstruction;
        set {
            SetField(ref currentInstruction, value);
            OnPropertyChanged(nameof(IsFirstInstruction));
            OnPropertyChanged(nameof(IsLastInstruction));
        }
    }
    
    public RelayCommand NextInstructionCommand => new(_ => ChangeInstruction());
    public RelayCommand BackInstructionCommand => new(_ => ChangeInstruction(false));
    public RelayCommand ShowFullBankCommand => new(_ => GetPlayerPageDataContext<InfoVM>().ToggleFullBankVisibility(true));
    public RelayCommand HideFullBankVisibleCommand => new(_ => GetPlayerPageDataContext<InfoVM>().ToggleFullBankVisibility(false));
    
    /// <summary>
    /// Переходит к правилам игры
    /// </summary>
    public RelayCommand StartRoundCommand => new(_ => ChangeMWPage<RegularRoundPanelPage>());
    
    public NextRoundInstructionVM() {
        Instructions = Instructions.Where(x => !string.IsNullOrEmpty(x)).ToList();
        CurrentInstruction = Instructions[currentInstructionIndex];
        ChangePWPage<InfoPage>();
    }

    /// <summary>
    /// Поменять страницу правил
    /// </summary>
    /// <param name="forward">Направление изменения страницы. По умолчанию - True, то есть следующая</param>
    private void ChangeInstruction(bool forward = true) {
        CurrentInstruction = Instructions[forward ? ++currentInstructionIndex : --currentInstructionIndex];
    }
}