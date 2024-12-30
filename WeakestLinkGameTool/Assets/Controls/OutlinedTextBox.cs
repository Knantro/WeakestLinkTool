using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WeakestLinkGameTool.Assets.Controls;

public class OutlinedTextBox : TextBox {
    private Pen _Pen;

    public OutlinedTextBox() {
        UpdatePen();
        TextChanged += (sender, args) => {
            UpdatePen();
        };
    }
    
    private void UpdatePen() {
        _Pen = new Pen(Brushes.Black, 1) {
            DashCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round,
            StartLineCap = PenLineCap.Round
        };

        InvalidateVisual();
    }
    
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        // var rect = GetRectFromCharacterIndex(CaretIndex).Location;

        var formattedText = new FormattedText(
            GetTextByLines(),
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
            FontSize,
            Brushes.Black);
        
        // Настройка цвета обводки и толщины
        var geometry = formattedText.BuildGeometry(new Point(0, 0));
        
        // var nIndex = Text.LastIndexOf('\n');
        // nIndex = nIndex == -1 ? 0 : nIndex;
        //
        // var start = GetRectFromCharacterIndex(nIndex).Location;
        // var end = GetRectFromCharacterIndex(Text.Length - 1).Location;
        //
        // if (nIndex == 0) {
        //     
        // }
        
        drawingContext.DrawGeometry(null, _Pen, geometry);
        drawingContext.DrawGeometry(Brushes.White, null, geometry);

        // var args = new CustomEventArgs {
        //     Args = new OutlinedTextBoxSource {
        //         FormattedText = new FormattedText(
        //             Text.Substring(0, CaretIndex + 1),
        //             CultureInfo.CurrentUICulture,
        //             FlowDirection.LeftToRight,
        //             new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
        //             FontSize,
        //             Brushes.Black)
        //     }
        // };
        //
        // OnSelectionChanged(args);
        
        // (GetTemplateChild("PART_ContentHost") as ScrollViewer);
    }

    private string GetTextByLines() {
        if (string.IsNullOrEmpty(Text)) return string.Empty;
        
        var result = Text;

        var currLength = 0;
        for (var i = 0; i < LineCount - 1; i++) {
            result = result.Insert(currLength += GetLineLength(i), Environment.NewLine);
        }

        return result;
    }
    
    public class CustomEventArgs : RoutedEventArgs {
        public object Args { get; set; }
        
        public CustomEventArgs() {
            
        }
    }
}