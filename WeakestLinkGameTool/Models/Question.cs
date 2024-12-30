using WeakestLinkGameTool.Models.Interfaces;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Вопрос игры
/// </summary>
public class Question : ITextUsable {
    
    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Текст ответа
    /// </summary>
    public string Answer { get; set; }
    
    /// <summary>
    /// Является ли вопросом финального раунда
    /// </summary>
    /// <remarks>
    /// Вопросы финального раунда не ограничены по времени и более сложные для ответа
    /// </remarks>
    public bool IsFinal { get; set; }
    
    /// <summary>
    /// Был ли использован этот вопрос ранее
    /// </summary>
    public bool IsUsed { get; set; }
}