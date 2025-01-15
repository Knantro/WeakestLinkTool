using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class SettingsVM : ViewModelBase {
    private bool settingSelected;
    private bool resolutionSettingSelected;
    private bool volumeSettingSelected;
    private double volume;
    
    public RelayCommand<string> ChangeResolutionCommand => new(ChangeResolution);
    public RelayCommand<string> SelectSettingCommand => new(SelectSetting);
    public RelayCommand ReturnCommand => new(_ => ReturnToSettingSelection());
    public RelayCommand BackCommand => new(_ => GoToMainMenu());

    /// <summary>
    /// 
    /// </summary>
    public bool SettingSelected {
        get => settingSelected;
        set => SetField(ref settingSelected, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ResolutionSettingSelected {
        get => resolutionSettingSelected;
        set => SetField(ref resolutionSettingSelected, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool VolumeSettingSelected {
        get => volumeSettingSelected;
        set => SetField(ref volumeSettingSelected, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public double Volume {
        get => volume;
        set {
            SetField(ref volume, value);
            SoundManager.SetVolume(SoundName.GENERAL_BED, (float)value / 100);
        }
    }

    public SettingsVM() {
        volume = mainWindowViewModel.Volume * 100;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting"></param>
    private void SelectSetting(string setting) {
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
    /// 
    /// </summary>
    private void ReturnToSettingSelection() {
        if (VolumeSettingSelected) {
            SoundManager.Stop(SoundName.GENERAL_BED);
            SetVolume((float)Volume / 100);
        }
        
        SettingSelected = false;
        ResolutionSettingSelected = false;
        VolumeSettingSelected = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newVolume"></param>
    private void SetVolume(float newVolume) {
        mainWindowViewModel.ChangeVolume(newVolume);
    }
    
    /// <summary>
    /// Меняет разрешение экрана
    /// </summary>
    /// <param name="res">Разрешение экрана</param>
    private void ChangeResolution(string res) {
        if (res == "Full") {
            mainWindowViewModel.TakeScreenToFull();
            return;
        }

        var resolution = Resolution.Parse(res);

        mainWindowViewModel.ChangeResolution(resolution);
    }
}