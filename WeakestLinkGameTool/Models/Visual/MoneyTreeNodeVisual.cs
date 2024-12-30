using System.Windows;
using System.Windows.Media;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Models.Visual;

public class MoneyTreeNodeVisual : INPCBase  {
    
    /// <summary>
    /// 
    /// </summary>
    public int Value { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string ValueString => Value.ToString();

    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Thickness Margin => InChain 
        ? ChainNumber == 1 
            ? new Thickness(0, 5, 0, 5) 
            : new Thickness(0, MarginCoefficient * (ChainNumber - 1) + 5, 0, -MarginCoefficient * (ChainNumber - 1) + 5)
        : new Thickness(0, 5, 0, 5);

    public uint MarginCoefficient { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int ChainNumber { get; set; }

    private bool inChain;

    /// <summary>
    /// 
    /// </summary>
    public bool InChain {
        get => inChain;
        set {
            SetField(ref inChain, value);
            OnPropertyChanged(nameof(Margin));
        }
    }

    private bool isActive;

    /// <summary>
    /// 
    /// </summary>
    public bool IsActive {
        get => isActive;
        set {
            SetField(ref isActive, value);
            OnPropertyChanged(nameof(Background));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Brush Background => IsActive 
        ? (LinearGradientBrush)Application.Current.Resources["RedButtonBrush"] 
        : (LinearGradientBrush)Application.Current.Resources["BlueButtonBrush"];

    /// <summary>
    /// 
    /// </summary>
    public double FontSize { get; set; }

    public MoneyTreeNodeVisual() {
        FontSize = 50;
        MarginCoefficient = 60;
        Height = ChainNumber == 8 ? 108 : 90;
        Width = ChainNumber == 8 ? 288 : 240;
    }
}