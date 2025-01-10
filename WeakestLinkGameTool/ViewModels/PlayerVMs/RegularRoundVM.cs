using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class RegularRoundVM : ViewModelBase {
    private TimeSpan timeLeft;
    private int bank;
    
    public event EventHandler<bool> RoundPanelVisibilityChanged;

    /// <summary>
    /// Денежная цепь
    /// </summary>
    public ObservableCollection<MoneyTreeNodeVisual> MoneyTree { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan TimeLeft {
        get => timeLeft;
        set {
            SetField(ref timeLeft, value);
            OnPropertyChanged(nameof(TimeText));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public string TimeText => timeLeft.ToString("m\\:ss");
    
    public RegularRoundVM() {
        // Реверс для отображения
        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;
        
        TimeLeft = WeakestLinkLogic.CurrentSession.CurrentRound.Timer!.Value;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void ShowRoundPanel() {
        RoundPanelVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideRoundPanel() {
        RoundPanelVisibilityChanged?.Invoke(this, false);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void TimerTick() {
        TimeLeft -= TimeSpan.FromSeconds(1);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public async Task MarkCorrectAnswer() {
        await UIDispatcherInvokeAsync(async () => {
            var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
            if (chainIndex != -1) {
                MoneyTree[chainIndex].InChain = true;
                await Task.Delay(200);
                MoneyTree[chainIndex].IsActive = false;
                if (chainIndex > 0) {
                    MoneyTree[chainIndex - 1].IsActive = true;
                }
            }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    public void MarkWrongAnswer() {
        ResetMoneyChain();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void BankMoney(int money) {
        Bank += money;
        ResetMoneyChain();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResetMoneyChain() {
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }
}