using System.Diagnostics;
using System.IO;

namespace WeakestLinkGameTool.Logic; 

/// <summary>
/// Пути к хранилищу информации по типам
/// </summary>
public static class FilePaths {
    public const string GAME_STATISTICS_TABLE = "session_statistics.xlsx";
    public const string QUESTIONS = "questions.json";
    public const string JOKES = "jokes.json";
    public const string SETTINGS = "settings.json";
    public const string APPSETTINGS = "appsettings.json";
    public const string SESSIONS_FOLDER = "Sessions";

    /// <summary>
    /// Возвращает полный путь до файла статистики игровой сессии с определённым номером
    /// </summary>
    /// <param name="sessionNumber">Номер игровой сессии</param>
    /// <returns>Абсолютный путь до файла статистики игровой сессии с определённым номером</returns>
    public static string GetSessionStatisticsFilePath(int sessionNumber) => 
        GetFullDataPath(Path.Combine(SESSIONS_FOLDER, $"[{sessionNumber}] {GAME_STATISTICS_TABLE}"));

    /// <summary>
    /// Возвращает массив из путей до файлов музыки игры
    /// </summary>
    /// <returns>Абсолютные пути до файлов музыки</returns>
    public static string[] GetSoundPaths() => Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Sounds"));

    /// <summary>
    /// Получить полный путь до файла с хранилищем информации определённого типа
    /// </summary>
    /// <param name="fileName">Имя файла (рекомендуется брать из полей <see cref="FilePaths"/> типа)</param>
    /// <returns>Абсолютный путь до файла</returns>
    public static string GetFullDataPath(string fileName) =>
        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Data", fileName);

    /// <summary>
    /// Получить полный путь до файла с хранилищем информации определённого типа
    /// </summary>
    /// <param name="fileName">Имя файла (рекомендуется брать из полей <see cref="FilePaths"/> типа)</param>
    /// <returns>Абсолютный путь до файла</returns>
    public static string GetFullPath(string fileName) =>
        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), fileName);
}