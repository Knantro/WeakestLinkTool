using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class GameRulesVM : ViewModelBase {
    private int bank;
    private bool isRoundPanelVisible;
    private CancellationTokenSource cts = new();
    private CancellationToken ct;
    private bool demoIsRunning;
    private bool canChangeChain = true;

    public event EventHandler<bool> GameRulesVisibilityChanged;

    /// <summary>
    /// Денежная цепь
    /// </summary>
    public ObservableCollection<MoneyTreeNodeVisual> MoneyTree { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundPanelVisible {
        get => isRoundPanelVisible;
        set => SetField(ref isRoundPanelVisible, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    public GameRulesVM() {
        ct = cts.Token;

        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowRoundPanel() {
        GameRulesVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideRoundPanel() {
        GameRulesVisibilityChanged?.Invoke(this, false);
    }

    private void ResetMoneyChain() {
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void WrongAnswerDemo() {
        SpinWait.SpinUntil(() => canChangeChain);
        canChangeChain = false;
        
        ResetMoneyChain();
        
        canChangeChain = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void BankDemo() {
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
    /// 
    /// </summary>
    public async Task StartDemo() {
        if (demoIsRunning) return;

        demoIsRunning = true;

        try {
            await Task.Run(async () => {
                while (!cts.IsCancellationRequested) {
                    await Task.Delay(1000);
                    SpinWait.SpinUntil(() => canChangeChain);
                    canChangeChain = false;
                    
                    var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
                    if (chainIndex != -1) {
                        MoneyTree[chainIndex].InChain = true;
                        await Task.Delay(200);
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
    /// 
    /// </summary>
    public void StopDemo() {
        if (!demoIsRunning) return;

        demoIsRunning = false;

        cts.Cancel();
        cts.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Dispose() {
        cts.Cancel();
        cts.Dispose();
    }
}