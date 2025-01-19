namespace WeakestLinkGameTool.Models.Statistics;

/// <summary>
/// Статистика игрока за игру
/// </summary>
public class FullPlayerStatistics {
    /// <summary>
    /// Игрок
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// Количество верных ответов
    /// </summary>
    public int CorrectAnswers { get; set; }

    /// <summary>
    /// Количество неверных ответов
    /// </summary>
    public int WrongAnswers { get; set; }

    /// <summary>
    /// Количество денег, положенных игроком в банк
    /// </summary>
    public int BankedMoney { get; set; }

    /// <summary>
    /// Средняя скорость ответов
    /// </summary>
    /// <remarks>
    /// Вернёт null, если не было ни одного ответа за раунд
    /// </remarks>
    public double? AverageSpeed { get; set; }

    /// <summary>
    /// Сколько раз игрок был 'сильным звеном' по статистике
    /// </summary>
    public int StrongestLinkCount { get; set; }

    /// <summary>
    /// Сколько раз игрок был 'слабым звеном' по статистике
    /// </summary>
    public int WeakestLinkCount { get; set; }

    /// <summary>
    /// Ответы на вопросы финала по порядку (true - игрок дал верный ответ, false - игрок дал неверный ответ)
    /// </summary>
    /// <remarks>
    /// Заполняется, если игрок был финалистом
    /// </remarks>
    public List<bool> FinalRoundAnswers { get; set; }
}