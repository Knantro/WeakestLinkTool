using System.Windows.Controls;

namespace WeakestLinkGameTool.Views.MainPages;

public partial class SettingsPage : UserControl {
    public SettingsPage() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };

        InitializeComponent();
    }

    private void ForceFocus(object sender, EventArgs args) => Focus();
}