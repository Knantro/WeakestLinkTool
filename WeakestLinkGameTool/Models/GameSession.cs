using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Игровая сессия
/// </summary>
public class GameSession {
    
    /// <summary>
    /// Глобальный идентификатор игровой сессии
    /// </summary>
    public Guid SessionID { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Список игроков текущей сессии
    /// </summary>
    public List<Player> AllPlayers { get; set; } = [];

    /// <summary>
    /// Список ещё не исключённых игроков сессии
    /// </summary>
    public List<Player> ActivePlayers => AllPlayers.Where(x => !x.IsKicked).ToList();
    
    /// <summary>
    /// Победитель игры
    /// </summary>
    public Player Winner { get; set; }
    
    /// <summary>
    /// Первый финалист игры
    /// </summary>
    public Player FirstFinalist { get; set; }
    
    /// <summary>
    /// Второй финалист игры
    /// </summary>
    public Player SecondFinalist { get; set; }

    /// <summary>
    /// Текущий раунд игры
    /// </summary>
    public Round CurrentRound { get; set; }

    /// <summary>
    /// Итоговое количество денег в банке за всю сессию игры 
    /// </summary>
    public int FullBank { get; set; }

    /// <summary>
    /// Раунды игры
    /// </summary>
    public List<Round> Rounds { get; } = [];

    /// <summary>
    /// Формирует полную статистику за игру
    /// </summary>
    /// <returns>Полная статистика игры</returns>
    public List<FullPlayerStatistics> GetFullGameStatistics() {
        var result = AllPlayers.Select(p => new FullPlayerStatistics {
            Player = p,
            BankedMoney = p.Statistics.Sum(x => x.BankedMoney),
            CorrectAnswers = p.Statistics.Sum(x => x.CorrectAnswers),
            WrongAnswers = p.Statistics.Sum(x => x.WrongAnswers),
            AverageSpeed = p.Statistics.Where(x => x.AverageSpeed != null).Average(x => x.AverageSpeed),
            WeakestLinkCount = p.Statistics.Count(x => x.IsWeakestLink),
            StrongestLinkCount = p.Statistics.Count(x => x.IsStrongestLink),
            FinalRoundAnswers = p.Statistics.FirstOrDefault(x => x.FinalRoundAnswers.IsAny())?.FinalRoundAnswers ?? []
        });

        return result.ToList();
    }
}