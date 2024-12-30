namespace WeakestLinkGameTool.Models.Interfaces;

/// <summary>
/// Интерфейс используемой текстовой сущности
/// </summary>
public interface ITextUsable {
    
    /// <summary>
    /// Текст сущности
    /// </summary>
    string Text { get; set; }
    
    /// <summary>
    /// Использована ли сущность
    /// </summary>
    bool IsUsed { get; set; }
}