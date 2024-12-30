using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class SettingsVM : ViewModelBase {
    
    public RelayCommand<string> ChangeResolutionCommand => new(ChangeResolution);
    public RelayCommand BackCommand => new(_ => GoToMainMenu());
    
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