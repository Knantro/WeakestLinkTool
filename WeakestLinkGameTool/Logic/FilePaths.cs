using System.Diagnostics;
using System.IO;

namespace WeakestLinkGameTool.Logic; 

/// <summary>
/// Пути к хранилищу информации по типам
/// </summary>
public static class FilePaths {
    public const string TITLES = "titles.txt";
    public const string GAME_SESSION_TEMPLATE = "game_session_*.txt";
    public const string GAME_SESSIONS = "game_sessions.json";
    public const string GAME_STATISTICS = "statistics.xlsx";
    public const string QUESTIONS = "questions.json";
    public const string JOKES = "jokes.json";
    public const string SETTINGS = "settings.json";
    public const string APPSETTINGS = "appsettings.json";

    /// <summary>
    /// Возвращает полный путь до файла истории игровой сессии с определённым номером
    /// </summary>
    /// <param name="sessionNumber">Порядковый номер игровой сессии</param>
    /// <returns>Абсолютный путь до файла истории игровой сессии с определённым номером</returns>
    public static string GetSessionFilePath(int sessionNumber) => 
        GetFullDataPath(GAME_SESSION_TEMPLATE.Replace("*", sessionNumber.ToString()));

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