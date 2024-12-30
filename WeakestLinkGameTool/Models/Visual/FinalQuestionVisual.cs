using System.Windows;
using System.Windows.Media;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Models.Visual;

public class FinalQuestionVisual : INPCBase {
    
    private bool isActive;
    private bool? isRight;
    private bool? isWrong;
    
    /// <summary>
    /// 
    /// </summary>
    public int QuestionNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsActive {
        get => isActive;
        set {
            SetField(ref isActive, value);
            OnPropertyChanged(nameof(Foreground));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool? IsRight {
        get => isRight;
        set {
            SetField(ref isRight, value);
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Foreground));
            OnPropertyChanged(nameof(Text));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool? IsWrong {
        get => isWrong;
        set {
            SetField(ref isWrong, value);
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Foreground));
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(FontSize));
        }
    }
    
    public double FontSize {
        get {
            if (IsWrong == true) return 38;
            return 50;
        }
    }
    
    public Thickness Padding {
        get {
            if (IsRight == true) return new Thickness(0, -6, 0, 6);
            return new Thickness(0);
        }
    }
    
    public string Text {
        get {
            if (IsRight == true) return "\u2714";
            if (IsWrong == true) return "\u274c";
            return QuestionNumber.ToString();
        }
    }

    public Brush Foreground => IsActive || IsRight == true || IsWrong == true
        ? Brushes.White
        : new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF));

    public Brush Background {
        get {
            if (IsRight == true) return (LinearGradientBrush)App.Current.Resources["GreenButtonBrush"];
            if (IsWrong == true) return (LinearGradientBrush)App.Current.Resources["RedButtonBrush"];
            return (LinearGradientBrush)App.Current.Resources["BlueButtonBrush"];
        }
    }
}