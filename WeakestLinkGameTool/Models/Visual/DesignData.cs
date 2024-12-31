using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models.Interfaces;
using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Models.Visual;

public static class DesignData {
    public static Player Player1 => new() { Number = 1, Name = "Ваня" };
    public static Player Player2 => new() { Number = 2, Name = "Дима" };

    public static TimeOnly Time => new(0, 3, 0);
    public static int Bank => 5000;

    public static ObservableCollection<MoneyTreeNodeVisual> MoneyTree => [
        new() { Value = 1000, ChainNumber = 1, Height = 90, Width = 240, InChain = true, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 2000, ChainNumber = 2, Height = 90, Width = 240, InChain = true, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 5000, ChainNumber = 3, Height = 90, Width = 240, InChain = true, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 10000, ChainNumber = 4, Height = 90, Width = 240, InChain = false, MarginCoefficient = 60, FontSize = 50, IsActive = true },
        new() { Value = 20000, ChainNumber = 5, Height = 90, Width = 240, InChain = false, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 30000, ChainNumber = 6, Height = 90, Width = 240, InChain = false, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 40000, ChainNumber = 7, Height = 90, Width = 240, InChain = false, MarginCoefficient = 60, FontSize = 50, IsActive = false },
        new() { Value = 50000, ChainNumber = 8, Height = 108, Width = 288, InChain = false, MarginCoefficient = 60, FontSize = 60, IsActive = false },
    ];

    public static ObservableCollection<Question> Questions => [
        new() { Text = "Сколько будет 232 умножить на 0?", Answer = "0", IsFinal = false, IsUsed = false },
        new() { Text = "Что больше, сантиметр или дюйм?", Answer = "Дюйм", IsFinal = false, IsUsed = false },
        new() { Text = "Назовите столицу Литвы", Answer = "Вильнюс", IsFinal = false, IsUsed = false },
    ];

    public static ObservableCollection<Question> FinalQuestions => [
        new() {
            Text = "О какой войне шла речь, которая была названа \"маленькой победоносной войной\" государственным деятелем Вячеславом ПлЕве и которую Россия проиграла?", Answer = "Русско-Японская война (1904-1905)", IsFinal = true, IsUsed = false
        },
        new() { Text = "В какие даты может отмечаться день программиста?", Answer = "12 и 13 сентября", IsFinal = true, IsUsed = false },
        new() { Text = "Каким событием закончилось татаро-монгольское иго, длившейся более двух веков?", Answer = "Стояние на реке Угре", IsFinal = true, IsUsed = false },
    ];

    public static ObservableCollection<Joke> Jokes => [
        new() { Text = "Кто заблудился в трёх соснах?", IsUsed = false },
        new() { Text = "У кого IQ отрицательный?", IsUsed = false },
        new() { Text = "Кому пора на выход?", IsUsed = false },
    ];

    public static ObservableCollection<Player> Players => [
        new() { Number = 1, Name = "Ваня", VotesCount = 1 },
        new() { Number = 2, Name = "Дима", VotesCount = 0 },
        new() { Number = 3, Name = "Петя", VotesCount = 2 },
        new() { Number = 4, Name = "Саша", VotesCount = 2 },
        new() { Number = 5, Name = "Толя", VotesCount = 0 },
    ];

    public static ObservableCollection<PlayerStatistics> PlayerStatistics => [
        new() {
            RoundName = "1",
            AnswerSpeeds = { 3.19, 5.17, 2.1, 4.5 },
            Player = new Player { Number = 1, Name = "Коля" },
            BankedMoney = 4000,
            CorrectAnswers = 2,
            IsStrongestLink = false,
            IsWeakestLink = false,
            WrongAnswers = 2
        },
        new() {
            RoundName = "2",
            AnswerSpeeds = { 2.12, 4.2, 3.51 },
            Player = new Player { Number = 1, Name = "Коля" },
            BankedMoney = 1000,
            CorrectAnswers = 2,
            IsStrongestLink = true,
            IsWeakestLink = false,
            WrongAnswers = 1
        },
        new() {
            RoundName = "3",
            AnswerSpeeds = { 7.49, 3.22, 9.39 },
            Player = new Player { Number = 1, Name = "Коля" },
            BankedMoney = 8000,
            CorrectAnswers = 0,
            IsStrongestLink = false,
            IsWeakestLink = true,
            WrongAnswers = 3
        },
    ];

    public static RoundStatistics RoundStatistics => new() {
        RoundNumber = 1,
        PlayersStatistics = {
            {
                new Player { Number = 1, Name = "Ваня" },
                new PlayerStatistics {
                    RoundName = "1",
                    AnswerSpeeds = { 5.59, 3.92, 8.3 },
                    Player = new Player { Number = 1, Name = "Ваня" },
                    BankedMoney = 4000,
                    CorrectAnswers = 2,
                    IsStrongestLink = true,
                    IsWeakestLink = false,
                    WrongAnswers = 1
                }
            }, {
                new Player { Number = 2, Name = "Дима" },
                new PlayerStatistics {
                    RoundName = "1",
                    AnswerSpeeds = { 4.53, 2.89, 6.1 },
                    Player = new Player { Number = 2, Name = "Дима" },
                    BankedMoney = 1000,
                    CorrectAnswers = 0,
                    IsStrongestLink = false,
                    IsWeakestLink = false,
                    WrongAnswers = 3
                }
            }, {
                new Player { Number = 3, Name = "Петя" },
                new PlayerStatistics {
                    RoundName = "1",
                    AnswerSpeeds = { 2.31, 6.7, 10.483, 2.1 },
                    Player = new Player { Number = 3, Name = "Петя" },
                    BankedMoney = 0,
                    CorrectAnswers = 2,
                    IsStrongestLink = false,
                    IsWeakestLink = false,
                    WrongAnswers = 2
                }
            }, {
                new Player { Number = 4, Name = "Саша" },
                new PlayerStatistics {
                    RoundName = "1",
                    AnswerSpeeds = { 5.81, 4.99, 3.12 },
                    Player = new Player { Number = 4, Name = "Саша" },
                    BankedMoney = 1000,
                    CorrectAnswers = 2,
                    IsStrongestLink = false,
                    IsWeakestLink = false,
                    WrongAnswers = 1
                }
            }, {
                new Player { Number = 5, Name = "Толя" },
                new PlayerStatistics {
                    RoundName = "1",
                    AnswerSpeeds = { 6.7, 2.133, 3.75, 4.94 },
                    Player = new Player { Number = 5, Name = "Толя" },
                    BankedMoney = 5000,
                    CorrectAnswers = 1,
                    IsStrongestLink = false,
                    IsWeakestLink = true,
                    WrongAnswers = 3
                }
            },
        }
    };
}