using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока с правилами игры
/// </summary>
public class GameRulesVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const int DEMO_CORRECT_ANSWER_DELAY = 1000;
    private const int CHAIN_ANIMATION_DELAY = 200;

    private int bank;
    private bool isRoundPanelVisible;
    private CancellationTokenSource cts = new();
    private CancellationToken ct;
    private bool demoIsRunning;
    private bool canChangeChain = true;

    /// <summary>
    /// Событие изменения видимости демо панели раунда игры
    /// </summary>
    public event EventHandler<bool> GameRulesVisibilityChanged;

    /// <summary>
    /// Денежная цепь
    /// </summary>
    public ObservableCollection<MoneyTreeNodeVisual> MoneyTree { get; set; }

    /// <summary>
    /// Банк демо
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    public GameRulesVM() {
        logger.SignedDebug();
        ct = cts.Token;

        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;
    }

    /// <summary>
    /// Показывает панель демо раунда
    /// </summary>
    public void ShowRoundPanel() {
        logger.SignedDebug();
        GameRulesVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Скрывает панель демо раунда
    /// </summary>
    public void HideRoundPanel() {
        logger.SignedDebug();
        GameRulesVisibilityChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Сбрасывает демо цепочку ответов раунда
    /// </summary>
    private void ResetMoneyChain() {
        logger.SignedDebug();
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }

    /// <summary>
    /// Фиксирует неверный ответ демо
    /// </summary>
    public void WrongAnswerDemo() {
        logger.SignedDebug();
        SpinWait.SpinUntil(() => canChangeChain);
        canChangeChain = false;

        ResetMoneyChain();

        canChangeChain = true;
    }

    /// <summary>
    /// Добавляет в банк деньги демо
    /// </summary>
    public void BankDemo() {
        logger.SignedDebug();
        SpinWait.SpinUntil(() => canChangeChain);
        canChangeChain = false;

        var money = MoneyTree.FirstOrDefault(x => x.InChain)?.Value ?? 0;

        if (money > 0) {
            if (Bank + money >= MoneyTree.First().Value) {
                money = MoneyTree.First().Value - Bank;
            }

            Bank += money;
            ResetMoneyChain();
        }

        canChangeChain = true;
    }

    /// <summary>
    /// Начинает показ демо
    /// </summary>
    public async Task StartDemo() {
        logger.SignedDebug();
        if (demoIsRunning) return;

        demoIsRunning = true;

        try {
            await Task.Run(async () => {
                while (!cts.IsCancellationRequested) {
                    await Task.Delay(DEMO_CORRECT_ANSWER_DELAY);
                    SpinWait.SpinUntil(() => canChangeChain);
                    canChangeChain = false;

                    var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
                    if (chainIndex != -1) {
                        MoneyTree[chainIndex].InChain = true;
                        await Task.Delay(CHAIN_ANIMATION_DELAY);
                        MoneyTree[chainIndex].IsActive = false;
                        if (chainIndex > 0) {
                            MoneyTree[chainIndex - 1].IsActive = true;
                        }
                    }

                    canChangeChain = true;
                }
            }, ct);
        }
        finally {
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }
    }

    /// <summary>
    /// Останавливает показ демо
    /// </summary>
    public void StopDemo() {
        logger.SignedDebug();
        if (!demoIsRunning) return;

        demoIsRunning = false;

        cts.Cancel();
        cts.Dispose();
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public override void Dispose() {
        logger.SignedDebug();
        cts.Cancel();
        cts.Dispose();
    }
}