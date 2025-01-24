using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Interfaces;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана редактора
/// </summary>
public class EditorVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private ITextUsable selectedItem;

    /// <summary>
    /// Режим редактирования
    /// </summary>
    public EditorMode EditorMode { get; set; } = EditorMode.Question;

    /// <summary>
    /// Заголовок коллекции текстовых сущностей
    /// </summary>
    public string ListBoxHeader => EditorMode switch {
        EditorMode.Question => $"Вопросы: {DataCollection.Count}",
        EditorMode.FinalQuestion => $"Вопросы финала: {DataCollection.Count}",
        EditorMode.Joke => $"Подколки: {DataCollection.Count}",
    };

    /// <summary>
    /// Текст первичного ввода вопроса или подколки
    /// </summary>
    public string MainInputText => EditorMode == EditorMode.Joke ? "Текст подколки" : "Текст вопроса";

    /// <summary>
    /// Является ли текущий режим редактирования - режимом редактирования вопросов
    /// </summary>
    public bool IsQuestionEditMode => EditorMode is EditorMode.Question or EditorMode.FinalQuestion;

    /// <summary>
    /// Прозрачность текстовых полей
    /// </summary>
    public double TextBoxOpacity => SelectedItem == null ? 0.5 : 1;

    /// <summary>
    /// Доступен ли ввод в текстовых полях
    /// </summary>
    public bool HasSelectedItem => SelectedItem != null;

    /// <summary>
    /// Текущая выбранная текстовая сущность
    /// </summary>
    public ITextUsable SelectedItem {
        get => selectedItem;
        set {
            SetField(ref selectedItem, value);
            OnPropertyChanged(nameof(HasSelectedItem));
            OnPropertyChanged(nameof(TextBoxOpacity));
        }
    }

    /// <summary>
    /// Выбран ли режим редактирования вопросов регулярного раунда (<see cref="EditorMode.Question"/>)
    /// </summary>
    public bool IsQuestionsSelected { get; set; }

    /// <summary>
    /// Выбран ли режим редактирования вопросов финального раунда (<see cref="EditorMode.FinalQuestion"/>)
    /// </summary>
    public bool IsFinalQuestionsSelected { get; set; }

    /// <summary>
    /// Выбран ли режим редактирования вопросов подколок (<see cref="EditorMode.Joke"/>)
    /// </summary>
    public bool IsJokesSelected { get; set; }

    /// <summary>
    /// Коллекция текстовых сущностей
    /// </summary>
    public ObservableCollection<ITextUsable> DataCollection { get; set; } = [];

    public RelayCommand<string> ChangeEditorModeCommand => new(mode => ChangeEditorMode(Enum.Parse<EditorMode>(mode)), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand AddItemCommand => new(_ => AddItem(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand RemoveItemCommand => new(_ => RemoveItem(), _ => HasSelectedItem && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand SaveCommand => new(_ => SaveAll(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand BackCommand => new(_ => GoToMainMenu(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    public EditorVM() {
        logger.SignedDebug();
        ChangeEditorMode(EditorMode.Question);
        DataCollection = WeakestLinkLogic.RegularQuestions.Select(ITextUsable (x) => x).ToObservableCollection();
    }

    /// <summary>
    /// Меняет режим редактирования
    /// </summary>
    /// <param name="mode">Режим редактирования</param>
    private void ChangeEditorMode(EditorMode mode) {
        logger.Debug($"Change Editor Mode to {mode}");
        EditorMode = mode;
        DataCollection = EditorMode switch {
            EditorMode.Question => WeakestLinkLogic.RegularQuestions.Select(ITextUsable (x) => x).ToObservableCollection(),
            EditorMode.FinalQuestion => WeakestLinkLogic.FinalQuestions.Select(ITextUsable (x) => x).ToObservableCollection(),
            EditorMode.Joke => WeakestLinkLogic.Jokes.Select(ITextUsable (x) => x).ToObservableCollection(),
        };

        SelectedItem = null;

        OnPropertyChanged(nameof(DataCollection));
        OnPropertyChanged(nameof(MainInputText));
        OnPropertyChanged(nameof(IsQuestionEditMode));
        IsQuestionsSelected = EditorMode == EditorMode.Question;
        IsFinalQuestionsSelected = EditorMode == EditorMode.FinalQuestion;
        IsJokesSelected = EditorMode == EditorMode.Joke;
        OnPropertyChanged(nameof(IsQuestionsSelected));
        OnPropertyChanged(nameof(IsFinalQuestionsSelected));
        OnPropertyChanged(nameof(IsJokesSelected));
        OnPropertyChanged(nameof(ListBoxHeader));
    }

    /// <summary>
    /// Добавляет сущность в коллекцию
    /// </summary>
    private void AddItem() {
        logger.Debug($"Add new item with mode {EditorMode}");
        switch (EditorMode) {
            case EditorMode.Question:
                SelectedItem = new Question { Text = "Новый вопрос", Answer = "Ответ" };
                WeakestLinkLogic.RegularQuestions.Add((Question)SelectedItem);
                break;
            case EditorMode.FinalQuestion:
                SelectedItem = new Question { Text = "Новый вопрос", Answer = "Ответ", IsFinal = true };
                WeakestLinkLogic.FinalQuestions.Add((Question)SelectedItem);
                break;
            case EditorMode.Joke:
                SelectedItem = new Joke { Text = "Новая подколка" };
                WeakestLinkLogic.Jokes.Add((Joke)SelectedItem);
                break;
        }

        DataCollection.Add(SelectedItem);
        OnPropertyChanged(nameof(ListBoxHeader));
    }

    /// <summary>
    /// Удаляет сущность из коллекции
    /// </summary>
    private void RemoveItem() {
        logger.Debug($"Remove item with mode {EditorMode}");
        switch (EditorMode) {
            case EditorMode.Question:
                WeakestLinkLogic.RegularQuestions.Remove((Question)SelectedItem);
                break;
            case EditorMode.FinalQuestion:
                WeakestLinkLogic.FinalQuestions.Remove((Question)SelectedItem);
                break;
            case EditorMode.Joke:
                WeakestLinkLogic.Jokes.Remove((Joke)SelectedItem);
                break;
        }

        DataCollection.Remove(SelectedItem);
        SelectedItem = DataCollection.Any() ? DataCollection.Last() : null;
        OnPropertyChanged(nameof(ListBoxHeader));
    }

    /// <summary>
    /// Сохраняет вопросы и подколки в файлы
    /// </summary>
    private void SaveAll() {
        try {
            logger.Info("Saving all edited data");
            if (!WeakestLinkLogic.ValidateEditableData()) {
                logger.Warn("Validation failed");
                mainWindowViewModel.ShowMessageBox("Заполните все поля у вопросов (текст вопроса, правильный ответ) и подколок (текст подколки)", "Ошибка");
                return;
            }

            WeakestLinkLogic.SaveEditableData();
            mainWindowViewModel.ShowMessageBox("Данные успешно сохранены!", "Успешно");
        }
        catch (Exception e) {
            logger.Error(e, "Save data failed");
            mainWindowViewModel.ShowMessageBox($"Данные не сохранились. Подробности в логе по пути logs/{DateTime.Now:yyyy-MM-dd}.txt", "Ошибка");
        }
    }
}