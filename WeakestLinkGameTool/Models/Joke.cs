using WeakestLinkGameTool.Models.Interfaces;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Модель 'подколки' ведущего игроков
/// </summary>
public class Joke : ITextUsable {
    
    /// <summary>
    /// Текст 'подколки'
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Была ли 'подколка' использована ранее
    /// </summary>
    public bool IsUsed { get; set; }
}