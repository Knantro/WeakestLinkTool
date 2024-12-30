using System.IO;
using System.Text.Encodings.Web;

namespace WeakestLinkGameTool.Logic;

/// <summary>
/// Управляет хранилищем данных приложения
/// </summary>
public static class FileStorage {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Настройки сериализации
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        PropertyNamingPolicy = null,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Сохраняет данные в файл в формате json
    /// </summary>
    /// <param name="data">Данные для сохранения</param>
    /// <param name="path">Путь сохранения</param>
    /// <typeparam name="T">Тип данных для сохранения</typeparam>
    public static bool Save<T>(T data, string path) {
        try {
            logger.SignedInfo($"Save data to file. Path: {path ?? "<NULL>"}");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonSerializer.Serialize(data, JsonSerializerOptions));
            logger.SignedInfo("Save data successful!");
            return true;
        }
        catch (Exception e) {
            logger.SignedError(e, "Save data failed");
            return false;
        }
    }

    /// <summary>
    /// Загружает данные из файла json-формата
    /// </summary>
    /// <param name="path">Путь хранения данных для загрузки</param>
    /// <typeparam name="T">Тип данных для загрузки</typeparam>
    /// <returns>Выгруженные данные, если загрузка прошла успешно, иначе значение по умолчанию</returns>
    public static T Load<T>(string path) {
        try {
            logger.SignedInfo($"Load data from file: {Path.GetFileName(path) ?? "<NULL>"}");
            
            if (!File.Exists(path)) {
                logger.SignedWarn(message: "No data available to load");
                return default;
            }
            
            var result = JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonSerializerOptions);
            
            logger.SignedInfo("Load data successful!");
            return result;
        }
        catch (Exception e) {
            logger.SignedError(e, "Load data failed");
            return default;
        }
    }
}