using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Interfaces;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class EditorVM : ViewModelBase {
    
    private ITextUsable selectedItem;

    /// <summary>
    /// Режим редактирования
    /// </summary>
    public EditorMode EditorMode { get; set; } = EditorMode.Question;
    
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
    /// Коллекция текстовых сущностей
    /// </summary>
    public ObservableCollection<ITextUsable> DataCollection { get; set; } = [];
    
    public RelayCommand<string> ChangeEditorModeCommand => new(mode => ChangeEditorMode(Enum.Parse<EditorMode>(mode)));
    public RelayCommand AddItemCommand => new(_ => AddItem());
    public RelayCommand RemoveItemCommand => new(_ => RemoveItem());
    public RelayCommand SaveCommand => new(_ => SaveAll());
    public RelayCommand BackCommand => new(_ => GoToMainMenu());
    
    public EditorVM() {
        DataCollection = WeakestLinkLogic.RegularQuestions.Select(ITextUsable (x) => x).ToObservableCollection();
    }
    
    /// <summary>
    /// Меняет режим редактирования
    /// </summary>
    /// <param name="mode">Режим редактирования</param>
    private void ChangeEditorMode(EditorMode mode) {
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
    }
    
    /// <summary>
    /// Добавляет сущность в коллекцию
    /// </summary>
    private void AddItem() {
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
    }
    
    /// <summary>
    /// Удаляет сущность из коллекции
    /// </summary>
    private void RemoveItem() {
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
    }
    
    /// <summary>
    /// Сохраняет вопросы и подколки в файлы
    /// </summary>
    private void SaveAll() {
        try {
            if (!WeakestLinkLogic.ValidateEditableData()) {
                mainWindowViewModel.ShowMessageBox("Заполните все поля у вопросов (текст вопроса, правильный ответ) и подколок (текст подколки)", "Ошибка");
                return;
            }

            WeakestLinkLogic.SaveEditableData();
            mainWindowViewModel.ShowMessageBox("Данные успешно сохранены!", "Успешно");
        }
        catch {
            mainWindowViewModel.ShowMessageBox($"Данные не сохранились. Подробности в логе по пути logs/{DateTime.Now:yyyy-MM-dd}.txt", "Ошибка");
        }
    }
}