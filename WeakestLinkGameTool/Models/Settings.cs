using WeakestLinkGameTool.Models.Visual;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Настройки приложения
/// </summary>
public class Settings {
    
    /// <summary>
    /// Разрешение экрана
    /// </summary>
    public Resolution ScreenResolution { get; set; }
    
    /// <summary>
    /// Громкость музыки
    /// </summary>
    public float Volume { get; set; }
}