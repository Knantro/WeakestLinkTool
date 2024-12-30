using System.Runtime.CompilerServices;

namespace WeakestLinkGameTool.Extensions;

/// <summary>
/// Расширения для логгирования
/// </summary>
public static class LoggerExtensions {
    
    /// <summary>
    /// Записывает лог на уровне "Trace" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedTrace(this Logger logger, string message = "", [CallerMemberName] string name = null) {
        logger.Trace($"[{name}] {message}");
    }
    
    /// <summary>
    /// Записывает лог на уровне "Debug" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedDebug(this Logger logger, string message = "", [CallerMemberName] string name = null) {
        logger.Debug($"[{name}] {message}");
    }
    
    /// <summary>
    /// Записывает лог на уровне "Info" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedInfo(this Logger logger, string message = "", [CallerMemberName] string name = null) {
        logger.Info($"[{name}] {message}");
    }

    /// <summary>
    /// Записывает лог на уровне "Warn" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="ex">Исключение для записи в лог</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedWarn(this Logger logger, Exception ex = null, string message = "", [CallerMemberName] string name = null) {
        logger.Warn(ex, $"[{name}] {message}");
    }
    
    /// <summary>
    /// Записывает лог на уровне "Error" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="ex">Исключение для записи в лог</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedError(this Logger logger, Exception ex = null, string message = "", [CallerMemberName] string name = null) {
        logger.Error(ex, $"[{name}] {message}");
    }
    
    /// <summary>
    /// Записывает лог на уровне "Fatal" с указанием имени члена, вызвавшего метод логгирования
    /// </summary>
    /// <param name="logger">Исходный логгер</param>
    /// <param name="ex">Исключение для записи в лог</param>
    /// <param name="message">Сообщение</param>
    /// <param name="name">Имя члена, вызвавшего метод логгирования</param>
    public static void SignedFatal(this Logger logger, Exception ex = null, string message = "", [CallerMemberName] string name = null) {
        logger.Fatal(ex, $"[{name}] {message}");
    }
}