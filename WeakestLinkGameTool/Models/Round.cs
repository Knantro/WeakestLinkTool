using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Раунд игры
/// </summary>
public class Round {
    
    /// <summary>
    /// Порядковый номер раунда
    /// </summary>
    public int Number { get; set; }
    
    /// <summary>
    /// Время, отведённое на раунд
    /// </summary>
    /// <remarks>
    /// Принимает значение NULL в случае, если <see cref="IsFinal"/> равняется True
    /// </remarks>
    public TimeSpan? Timer { get; set; }

    /// <summary>
    /// Деньги, положенные в банк
    /// </summary>
    /// <remarks>
    /// Принимает значение NULL в случае, если <see cref="IsFinal"/> равняется True
    /// </remarks>
    public int? BankedMoney { get; set; } = 0;
    
    /// <summary>
    /// Является ли раунд предфинальным
    /// </summary>
    /// <remarks>
    /// В предфинальном раунде все положенные деньги в банк умножаются на 2
    /// </remarks>
    public bool IsPreFinal { get; set; }
    
    /// <summary>
    /// Является ли раунд финальным
    /// </summary>
    /// <remarks>
    /// В финальном раунде нет ни ограничений времени, ни денег в банке
    /// </remarks>
    public bool IsFinal { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Player KickedPlayer { get; set; }

    /// <summary>
    /// Статистика раунда
    /// </summary>
    public RoundStatistics Statistics { get; set; } = new();
}

/// <summary>
/// Финальный раунд игры
/// </summary>
public class FinalRound : Round {
    
    /// <summary>
    /// Первый игрок финала
    /// </summary>
    public Player FirstPlayer { get; set; }
    
    /// <summary>
    /// Второй игрок финала
    /// </summary>
    public Player SecondPlayer { get; set; }
    
    /// <summary>
    /// Ответы первого игрока (false - неправильный ответ, true - правильный ответ)
    /// </summary>
    public List<bool> FirstPlayerAnswers { get; set; }
    
    /// <summary>
    /// Ответы второго игрока (false - неправильный ответ, true - правильный ответ)
    /// </summary>
    public List<bool> SecondPlayerAnswers { get; set; }
    
    public FinalRound() {
        KickedPlayer = null;
        Timer = null;
        BankedMoney = null;
        IsPreFinal = false;
        IsFinal = true;
    }
}