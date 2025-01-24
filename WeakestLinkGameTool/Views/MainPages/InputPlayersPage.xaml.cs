using System.Windows.Controls;
using System.Windows.Input;

namespace WeakestLinkGameTool.Views.MainPages;

public partial class InputPlayersPage : UserControl {
    public InputPlayersPage() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };

        InitializeComponent();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            Focus();
        }
        
        base.OnPreviewKeyDown(e);
    }

    private void ForceFocus(object sender, EventArgs args) => Focus();
}