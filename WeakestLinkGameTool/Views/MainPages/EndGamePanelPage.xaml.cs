using System.Windows.Controls;

namespace WeakestLinkGameTool.Views.MainPages;

public partial class EndGamePanelPage : UserControl {
    public EndGamePanelPage() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };

        InitializeComponent();
    }

    private void ForceFocus(object sender, EventArgs args) => Focus();
}