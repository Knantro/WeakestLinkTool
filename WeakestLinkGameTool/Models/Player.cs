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
    private bool chosenForPersonalStatistics;
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
    /// Выбран ли данный игрок для формирования персональной статистики (для экрана конца игры)
    /// </summary>
    public bool ChosenForPersonalStatistics {
        get => chosenForPersonalStatistics;
        set => SetField(ref chosenForPersonalStatistics, value);
    }
    
    /// <summary>
    /// Количество голосов на голосовании за "Слабое звено"
    /// </summary>
    public int VotesCount {
        get => votesCount;
        set => SetField(ref votesCount, value);
    }
    
    /// <summary>
    /// Исключён ли данный игрок
    /// </summary>
    public bool IsKicked { get; set; }
    
    /// <summary>
    /// Является ли игрок победителем
    /// </summary>
    public bool IsWinner { get; set; }

    /// <summary>
    /// Статистики игрока по раундам
    /// </summary>
    public List<PlayerStatistics> Statistics { get; set; } = [];
}