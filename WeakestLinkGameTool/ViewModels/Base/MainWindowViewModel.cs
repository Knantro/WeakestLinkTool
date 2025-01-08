using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

namespace WeakestLinkGameTool.ViewModels.Base; 

/// <summary>
/// Главная модель-представления для всех страниц
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private static readonly WeakestLinkLogic WeakestLinkLogic = App.ServiceProvider.GetService<WeakestLinkLogic>();

    private Settings settings;
    private UserControl currentMWPage;
    private UserControl currentPWPage;
    private MessageBox messageBox;
    private bool isMessageBoxVisible;
    private double width = 1280 - 2 * SystemParameters.MinimizedWindowHeight + 2;
    private double height = 720;
    private WindowState windowState = WindowState.Normal;
    private WindowStyle windowStyle = WindowStyle.SingleBorderWindow;

    /// <summary>
    /// Событие закрытия диалогового окна
    /// </summary>
    public event DialogResultEventHandler OnDialogResult;
    
    /// <summary>
    /// Диалоговое окно
    /// </summary>
    public MessageBox MessageBox {
        get => messageBox;
        set => SetField(ref messageBox, value);
    }
    
    /// <summary>
    /// Видимость диалогового окна
    /// </summary>
    public bool IsMessageBoxVisible {
        get => isMessageBoxVisible;
        set => SetField(ref isMessageBoxVisible, value);
    }
    
    /// <summary>
    /// Текущая страница главного окна
    /// </summary>
    public UserControl CurrentMWPage {
        get => currentMWPage;
        set => SetField(ref currentMWPage, value);
    }
    
    /// <summary>
    /// Текущая страница окна игрока
    /// </summary>
    public UserControl CurrentPWPage {
        get => currentPWPage;
        set => SetField(ref currentPWPage, value);
    }
    
    /// <summary>
    /// Ширина экрана
    /// </summary>
    public double Width {
        get => width;
        set => SetField(ref width, value);
    }
    
    /// <summary>
    /// Высота экрана
    /// </summary>
    public double Height {
        get => height;
        set => SetField(ref height, value);
    }
    
    /// <summary>
    /// Состояние окна
    /// </summary>
    public WindowState WindowState {
        get => windowState;
        set => SetField(ref windowState, value);
    }
    
    /// <summary>
    /// Стиль окна
    /// </summary>
    public WindowStyle WindowStyle {
        get => windowStyle;
        set => SetField(ref windowStyle, value);
    }
    
    public RelayCommand<CancelEventArgs> ClosingCommand { get; }

    public MainWindowViewModel() {
        RestoreSettings();
        ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosingWindow);
    }

    public void OnClosingWindow(CancelEventArgs e) {
        e.Cancel = true;
        ShowMessageBox("Вы уверены, что хотите выйти из игры?", "Предупреждение", MessageBoxButton.YesNo);
        OnDialogResult += ClosingDialogResult;
    }

    private void ClosingDialogResult(MessageBoxResult result) {
        if (result == MessageBoxResult.Yes) {
            Application.Current.Shutdown();
        }

        OnDialogResult -= ClosingDialogResult;
    }

    /// <summary>
    /// Показывает диалоговое окно
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="caption">Заголовок</param>
    /// <param name="buttons">Кнопки</param>
    public void ShowMessageBox(string message, string caption = null, MessageBoxButton buttons = MessageBoxButton.OK)
    {
        MessageBox = new MessageBox {
            Caption = { Text = caption },
            MessageContainer = { Text = message }
        };
        
        MessageBox.AddButtons(buttons);

        IsMessageBoxVisible = true;
    }

    /// <summary>
    /// Скрывает диалоговое окно и вызывает событие закрытия диалогового окна
    /// </summary>
    /// <param name="result">Результат диалогового окна</param>
    public void RaiseDialogResult(MessageBoxResult result) {
        IsMessageBoxVisible = false;
        
        OnDialogResult?.Invoke(result);
    }

    /// <summary>
    /// Меняет разрешение экрана
    /// </summary>
    /// <param name="resolution">Разрешение экрана</param>
    public void ChangeResolution(Resolution resolution) {
        Height = resolution.Height;
        Width = resolution.Width - 2 * SystemParameters.MinimizedWindowHeight + 2;
        WindowStyle = WindowStyle.SingleBorderWindow;
        WindowState = WindowState.Normal;
        
        SaveSettings();
    }

    /// <summary>
    /// Переводит приложение в полноэкранный режим
    /// </summary>
    public void TakeScreenToFull() {
        Height = Screen.PrimaryScreen.Bounds.Height;
        Width = Screen.PrimaryScreen.Bounds.Width;
        WindowStyle = WindowStyle.None;
        WindowState = WindowState.Maximized;
        
        SaveSettings();
    }
    
    /// <summary>
    /// Восстанавливает сохранённые настройки приложения, если это возможно
    /// </summary>
    private void RestoreSettings() {
        var restoredSettings = FileStorage.Load<Settings>(FilePaths.GetFullDataPath(FilePaths.SETTINGS));

        if (restoredSettings is not null) {
            settings = restoredSettings;

            Width = settings.ScreenResolution.Width;
            Height = settings.ScreenResolution.Height;
            
            if (settings.ScreenResolution.IsFullScreenResolution()) {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
    }

    /// <summary>
    /// Сохраняет текущие настройки приложения
    /// </summary>
    private void SaveSettings() {
        settings ??= new Settings();
        settings.ScreenResolution = new Resolution(Width, Height);
        
        FileStorage.Save(settings, FilePaths.GetFullDataPath(FilePaths.SETTINGS));
    }

    #region INPC

    /// <summary>
    /// Событие, что свойство поменяло своё значение
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Обрабатывает событие изменения свойства
    /// </summary>
    /// <param name="propertyName">Имя свойства</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        logger.SignedTrace($"Called for {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Устанавливает значение свойства с вызовом события об его изменении
    /// </summary>
    /// <param name="field">Поддерживающее поле свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <typeparam name="T">Тип свойства</typeparam>
    /// <returns>True, если свойство поменяло значение, иначе False</returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
        logger.SignedTrace($"Called for {propertyName}");
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}