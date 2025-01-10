using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class FinalRoundInstructionVM : ViewModelBase {
        
    private int currentInstructionIndex = 0;
    private string currentInstruction;

    /// <summary>
    /// Страницы вступления
    /// </summary>
    public List<string> Instructions { get; set; } = [
        $"Вы двое в этом раунде заработали {(WeakestLinkLogic.CurrentSession.CurrentRound.BankedMoney!.Value / 2).Decline("рубль", "рубля", "рублей")}",
        "Ну что ж, мы удвоим эту сумму и добавим к тому, что было заработано в предыдущих раундах",
        $"Это значит, что общий приз сегодня составляет {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}",
        "Но только один из вас сможет забрать эти деньги с собой",
        "Сейчас вам придётся играть друг против друга. Лицом к лицу",
        "Вы по очереди ответите на 5 пар вопросов. Выиграет тот, кто даст больше правильных ответов. Правило очень простое",
        "Если после 5 пар вопросов у нас ничья, мы продолжаем игру до первого проигрыша, пока не определится победитель",
        $"Итак {WeakestLinkLogic.CurrentSession.ActivePlayers[0].Name}, {WeakestLinkLogic.CurrentSession.ActivePlayers[1].Name}, на кон поставлено {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}",
        "Играем в \"Слабое звено\"",
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
    public RelayCommand StartFinalCommand => new(_ => ChangeMWPage<FinalRoundPanelPage>());
    
    public FinalRoundInstructionVM() {
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