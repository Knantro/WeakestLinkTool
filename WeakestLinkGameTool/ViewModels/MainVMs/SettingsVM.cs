using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана настроек игры
/// </summary>
public class SettingsVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const float VOLUME_MODIFIER = 100;

    private bool settingSelected;
    private bool resolutionSettingSelected;
    private bool volumeSettingSelected;
    private double volume;

    public RelayCommand<string> ChangeResolutionCommand => new(ChangeResolution, _ => ResolutionSettingSelected && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand<string> SelectSettingCommand => new(SelectSetting, _ => !SettingSelected && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand ReturnCommand => new(_ => ReturnToSettingSelection(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand BackCommand => new(_ => GoToMainMenu(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Выбрана ли какая-то определённая настройка
    /// </summary>
    public bool SettingSelected {
        get => settingSelected;
        set => SetField(ref settingSelected, value);
    }

    /// <summary>
    /// Выбрана ли настройка изменения разрешения экрана
    /// </summary>
    public bool ResolutionSettingSelected {
        get => resolutionSettingSelected;
        set => SetField(ref resolutionSettingSelected, value);
    }

    /// <summary>
    /// Выбрана ли настройка изменения громкости звуков
    /// </summary>
    public bool VolumeSettingSelected {
        get => volumeSettingSelected;
        set => SetField(ref volumeSettingSelected, value);
    }

    /// <summary>
    /// Текущая громкость звуков
    /// </summary>
    public double Volume {
        get => volume;
        set {
            SetField(ref volume, value);
            SoundManager.SetVolume(SoundName.GENERAL_BED, (float)value / VOLUME_MODIFIER);
        }
    }

    public SettingsVM() {
        volume = mainWindowViewModel.Volume * VOLUME_MODIFIER;
    }

    /// <summary>
    /// Выбирает настройку для изменения
    /// </summary>
    /// <param name="setting">Настройка</param>
    private void SelectSetting(string setting) {
        logger.Debug($"Select setting: {setting}");
        switch (setting) {
            case "Resolution":
                ResolutionSettingSelected = true;
                SettingSelected = true;
                break;
            case "Volume":
                SoundManager.Play(SoundName.GENERAL_BED);
                VolumeSettingSelected = true;
                SettingSelected = true;
                break;
        }
    }

    /// <summary>
    /// Возвращает к выбору настройки для изменения
    /// </summary>
    private void ReturnToSettingSelection() {
        logger.Debug("Return to setting selection");
        if (VolumeSettingSelected) {
            logger.Debug($"Saving new volume {Volume / VOLUME_MODIFIER:F2}");
            SoundManager.Stop(SoundName.GENERAL_BED);
            SetVolume((float)Volume / VOLUME_MODIFIER);
        }

        SettingSelected = false;
        ResolutionSettingSelected = false;
        VolumeSettingSelected = false;
    }

    /// <summary>
    /// Устанавливает новое значение громкости звуков
    /// </summary>
    /// <param name="newVolume">Новая громкость</param>
    private void SetVolume(float newVolume) {
        mainWindowViewModel.ChangeVolume(newVolume);
    }

    /// <summary>
    /// Меняет разрешение экрана
    /// </summary>
    /// <param name="res">Разрешение экрана</param>
    private void ChangeResolution(string res) {
        logger.Debug($"Change resolution to: {res}");
        if (res == "Full") {
            mainWindowViewModel.TakeScreenToFull();
            return;
        }

        var resolution = Resolution.Parse(res);

        mainWindowViewModel.ChangeResolution(resolution);
    }
}