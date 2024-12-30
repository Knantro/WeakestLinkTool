using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Models; 

/// <summary>
/// Модель игрока
/// </summary>
public class Player : INPCBase {
    
    private int number;
    private string name;
    private bool isStrongestLink;
    private bool isWeakestLink;
    private int votesCount;
    
    /// <summary>
    /// Номер игрока
    /// </summary>
    public int Number {
        get => number;
        set => SetField(ref number, value);
    }
    
    /// <summary>
    /// Имя игрока
    /// </summary>
    public string Name {
        get => name;
        set => SetField(ref name, value);
    }
    
    /// <summary>
    /// Является ли игрок сильным звеном
    /// </summary>
    public bool IsStrongestLink  {
        get => isStrongestLink;
        set => SetField(ref isStrongestLink, value);
    }
    
    /// <summary>
    /// Является ли игрок слабым звеном
    /// </summary>
    public bool IsWeakestLink  {
        get => isWeakestLink;
        set => SetField(ref isWeakestLink, value);
    }
    
    /// <summary>
    /// Количество голосов на голосовании за "Слабое звено"
    /// </summary>
    public int VotesCount {
        get => votesCount;
        set {
            SetField(ref votesCount, value);
            OnPropertyChanged(nameof(VotesCountIsZero));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool VotesCountIsZero => VotesCount == 0;
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsKicked { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsWinner { get; set; }

    /// <summary>
    /// Статистики игрока по раундам
    /// </summary>
    public List<PlayerStatistics> Statistics { get; set; } = [];
}