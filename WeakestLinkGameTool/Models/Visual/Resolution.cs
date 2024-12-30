using System.Windows.Forms;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Models.Visual;

/// <summary>
/// Разрешение экрана
/// </summary>
public struct Resolution {
    
    /// <summary>
    /// Ширина экрана
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Высота экрана
    /// </summary>
    public double Height { get; set; }

    public Resolution(double width, double height) {
        Width = width;
        Height = height;
    }

    public bool IsFullScreenResolution() => Screen.PrimaryScreen.Bounds.Height == Height && Screen.PrimaryScreen.Bounds.Width == Width;

    /// <summary>
    /// Преобразует строку в разрешение экрана
    /// </summary>
    /// <param name="resolution">Разрешение экрана в строковом виде</param>
    /// <returns>Разрешение экрана</returns>
    /// <exception cref="ArgumentException">Если на вход подана строка не в формате WxH, где W - целочисленная ширина экрана, а H - целочисленная высота экрана</exception>
    public static Resolution Parse(string resolution) {
        var split = resolution.Split('x');
        if (split.Length != 2 || split.FirstOrDefault(x => x.Length == 0) != null
            || !double.TryParse(split[0], out var width) || !double.TryParse(split[1], out var height)) {
            throw new ArgumentException("String must have WxH format, where W - width in integer, H - height in integer");
        }

        return new Resolution { Width = width, Height = height };
    }
}