using System.Windows.Controls;

namespace WeakestLinkGameTool.Views.MainPages;

public partial class EditorPage : UserControl {
    public EditorPage() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };

        InitializeComponent();
    }

    private void ForceFocus(object sender, EventArgs args) => Focus();

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        (sender as ListBox).ScrollIntoView((sender as ListBox).SelectedItem);
    }
}