using System.Text;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class EndGameVM : ViewModelBase {
    private string credits;

    public event EventHandler CompleteCreditsRaised;

    /// <summary>
    /// 
    /// </summary>
    public string Credits {
        get => credits;
        set => SetField(ref credits, value);
    } 
    
    public EndGameVM() {
        BuildCredits();
    }

    /// <summary>
    /// 
    /// </summary>
    public void CompleteCreditsImmediately() {
        CompleteCreditsRaised?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    private void BuildCredits() {
        var sb = new StringBuilder();
        AddLine(sb, $"Победитель - {WeakestLinkLogic.CurrentSession.Winner.Name}", true);
        // AddLine(sb, $"Победитель - Александр", true); // TODO: Тест
        
        AddLine(sb, "Ведущий");
        AddLine(sb, "Knantro", true);
        
        AddLine(sb, "Автор программы");
        AddLine(sb, "Knantro", true);
        
        AddLine(sb, "Разработчики");
        AddLine(sb, "Knantro", true);
        
        AddLine(sb, "Дизайнеры");
        AddLine(sb, "Knantro", true);
        
        AddLine(sb, "Авторы вопросов");
        AddLine(sb, "Knantro");
        AddLine(sb, "Банк вопросов \"Слабого Звена\"");
        AddLine(sb, "NULL"); // TODO: Указать автора 
        AddLine(sb, "NULL", true); // TODO: Указать автора 
        
        AddLine(sb, "Авторы подколок");
        AddLine(sb, "Knantro");
        AddLine(sb, "Банк вопросов \"Слабого Звена\"");
        AddLine(sb, "NULL"); // TODO: Указать автора 
        AddLine(sb, "NULL", true); // TODO: Указать автора 
        
        AddLine(sb, "Музыка");
        AddLine(sb, "Paul Farrer", true);
        
        AddLine(sb, "Основано на");
        AddLine(sb, "телевизионном");
        AddLine(sb, "формате");
        AddLine(sb, "BBC \"The Weakest Link\"", true);
        
        Credits = sb.ToString();
    }

    private void AddLine(StringBuilder sb, string value, bool isEnd = false) {
        sb.Append(value);
        AppendLinesCredits(sb, isEnd ? 3 : 1);
    }

    private void AppendLinesCredits(StringBuilder sb, int count) {
        for (var i = 0; i < count; i++) {
            sb.AppendLine();
        }
    }
}