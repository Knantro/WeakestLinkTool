using System.Diagnostics;
using System.Windows.Controls;
using WeakestLinkGameTool.ViewModels.MainVMs;

namespace WeakestLinkGameTool.Views.MainPages;

public partial class EditorPage : UserControl {
    public EditorPage() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };

        InitializeComponent();
    }

    private void ForceFocus(object sender, EventArgs args) => Focus();

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var listBox = sender as ListBox;
        var item = (ListBoxItem)listBox!.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
        item?.Focus();
        listBox.ScrollIntoView(listBox.SelectedItem);
    }
}