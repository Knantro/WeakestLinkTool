namespace WeakestLinkGameTool.Models.Statistics;

/// <summary>
/// Статистика раунда
/// </summary>
public class RoundStatistics {
    
    /// <summary>
    /// Номер раунда
    /// </summary>
    public int RoundNumber { get; set; }

    /// <summary>
    /// Статистика игроков за текущий раунд
    /// </summary>
    public Dictionary<Player, PlayerStatistics> PlayersStatistics { get; set; } = [];
    
    /// <summary>
    /// Деньги, положенные в банк за текущий раунд
    /// </summary>
    public int BankedMoney { get; set; }
}