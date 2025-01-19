using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока 
/// </summary>
public class RegularRoundVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const int CHAIN_ANIMATION_DELAY = 200;

    private TimeSpan timeLeft;
    private int bank;

    public event EventHandler<bool> RoundPanelVisibilityChanged;

    /// <summary>
    /// Денежная цепь
    /// </summary>
    public ObservableCollection<MoneyTreeNodeVisual> MoneyTree { get; set; } = [];

    /// <summary>
    /// Текущий банк
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    /// <summary>
    /// Оставшиеся время раунда
    /// </summary>
    public TimeSpan TimeLeft {
        get => timeLeft;
        set {
            SetField(ref timeLeft, value);
            OnPropertyChanged(nameof(TimeText));
        }
    }

    /// <summary>
    /// Оставшиеся время в строковом формате
    /// </summary>
    public string TimeText => timeLeft.ToString("m\\:ss");

    public RegularRoundVM() {
        logger.SignedDebug();
        // Реверс для отображения
        MoneyTree = WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse().ToObservableCollection();
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;

        TimeLeft = WeakestLinkLogic.CurrentSession.CurrentRound.Timer!.Value;
    }

    /// <summary>
    /// Показывает панель раунда
    /// </summary>
    public void ShowRoundPanel() {
        logger.SignedDebug();
        RoundPanelVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Скрывает панель раунда
    /// </summary>
    public void HideRoundPanel() {
        logger.SignedDebug();
        RoundPanelVisibilityChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Обрабатывает прошедшую секунду раунда, отображая это на экраны ведущего и игрока
    /// </summary>
    public void TimerTick() {
        logger.SignedDebug();
        TimeLeft -= TimeSpan.FromSeconds(1);
    }

    /// <summary>
    /// Фиксирует верный ответ игрока
    /// </summary>
    public async Task MarkCorrectAnswer() {
        logger.SignedDebug();
        await UIDispatcherInvokeAsync(async () => {
            var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
            if (chainIndex != -1) {
                MoneyTree[chainIndex].InChain = true;
                await Task.Delay(CHAIN_ANIMATION_DELAY);
                MoneyTree[chainIndex].IsActive = false;
                if (chainIndex > 0) {
                    MoneyTree[chainIndex - 1].IsActive = true;
                }
            }
        });
    }

    /// <summary>
    /// Фиксирует неверный ответ игрока
    /// </summary>
    public void MarkWrongAnswer() {
        logger.SignedDebug();
        ResetMoneyChain();
    }

    /// <summary>
    /// Сохраняет накопленные в цепочке ответов деньги в банк
    /// </summary>
    public void BankMoney(int money) {
        logger.SignedDebug();
        Bank += money;
        ResetMoneyChain();
    }

    /// <summary>
    /// Сбрасывает текущую денежную цепь
    /// </summary>
    private void ResetMoneyChain() {
        logger.SignedDebug();
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }
}