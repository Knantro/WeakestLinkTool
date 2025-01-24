using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WeakestLinkGameTool.Models;

namespace WeakestLinkGameTool.Logic;

/// <summary>
/// Генератор файлов статистики игры в формате Excel
/// </summary>
public static class ExcelStatistics {
    private const float AUTOFIT_SCALE_FACTOR = 1.05f;
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static string fontName = "Cascadia Code";
    private static int headerFontSize = 13;
    private static int commonFontSize = 11;
    private static int crossSymbolFontSize = 9;
    private static Color greyColor = Color.FromArgb(198, 193, 185);
    private static Color greenColor = Color.FromArgb(0, 176, 80);
    private static Color redColor = Color.FromArgb(255, 0, 0);
    private static Color fullStatisticsTabColor = Color.FromArgb(0, 136, 136);
    private static Color roundStatisticsTabColor = Color.FromArgb(48, 84, 150);
    private static Color playerStatisticsTabColor = Color.FromArgb(112, 48, 160);
    private static string noAnswerSymbol = "\u2796";
    private static string correctAnswerSymbol = "\u2714";
    private static string wrongAnswerSymbol = "\u274c";

    static ExcelStatistics() {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <summary>
    /// Формирует файл статистики 
    /// </summary>
    /// <param name="session">Игровая сессия</param>
    public static void Generate(GameSession session) {
        var document = new ExcelPackage();
        logger.Info("Generating Excel Statistics...");

        AddFullGameStatistics(session, document);
        AddRoundStatistics(session, document);
        AddPlayerStatistics(session, document);

        File.WriteAllBytes(FilePaths.GetSessionStatisticsFilePath(session.SessionID), document.GetAsByteArray());
    }

    /// <summary>
    /// Добавляет в документ полную статистику игры
    /// </summary>
    /// <param name="session">Игровая сессия</param>
    /// <param name="document">Текущий документ</param>
    private static void AddFullGameStatistics(GameSession session, ExcelPackage document) {
        logger.Info("Generating Full Game Statistics...");
        try {
            var sheet = document.Workbook.Worksheets.Add("Статистика за всю игру");

            sheet.TabColor = fullStatisticsTabColor;

            var titleCells = sheet.Cells[1, 1, 1, 7];
            titleCells.Value = "Статистика игры";
            titleCells.Merge = true;
            titleCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            titleCells.Style.Font.SetFromFont(fontName, headerFontSize, true);

            var statisticsHeaderCells = sheet.Cells[2, 1, 2, 7];
            statisticsHeaderCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            statisticsHeaderCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            statisticsHeaderCells.Style.Font.SetFromFont(fontName, headerFontSize, true);
            statisticsHeaderCells.LoadFromArrays([["Имя", "Верно", "Неверно", "Банк", "Ср. ск. ответа", "Сильное звено", "Слабое звено"]]);

            var row = 2;
            var column = 1;
            foreach (var item in session.GetFullGameStatistics()) {
                row++;
                sheet.Cells[row, column].Value = item.Player.Name;
                sheet.Cells[row, column + 1].Value = item.CorrectAnswers;
                sheet.Cells[row, column + 2].Value = item.WrongAnswers;
                sheet.Cells[row, column + 3].Value = item.BankedMoney;
                sheet.Cells[row, column + 4].Value = item.AverageSpeed;
                sheet.Cells[row, column + 4].Style.Numberformat.Format = "#,##0.00";
                sheet.Cells[row, column + 5].Value = item.StrongestLinkCount;
                sheet.Cells[row, column + 6].Value = item.WeakestLinkCount;
            }

            var commonStatisticsCells = sheet.Cells[3, 1, row, 7];
            commonStatisticsCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            commonStatisticsCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            commonStatisticsCells.Style.Font.SetFromFont(fontName, commonFontSize);

            var max = Math.Max(
                session.FirstFinalist.Statistics.Last().FinalRoundAnswers.Count,
                session.SecondFinalist.Statistics.Last().FinalRoundAnswers.Count);

            var finalHeaderCells = sheet.Cells[1, 8, 1, 8 + max];
            finalHeaderCells.LoadFromArrays([new object[] { "Финал" }.Concat(Enumerable.Range(1, max).Select(x => (object)x)).ToArray()]);
            finalHeaderCells.Style.Font.SetFromFont(fontName, headerFontSize, true);
            finalHeaderCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            finalHeaderCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var firstFinalistCell = sheet.Cells["H2"];
            firstFinalistCell.Value = session.FirstFinalist.Name;
            firstFinalistCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            firstFinalistCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            firstFinalistCell.Style.Font.SetFromFont(fontName, commonFontSize);
            if (session.FirstFinalist.IsWinner) {
                firstFinalistCell.Style.Font.Color.SetColor(greenColor);
                firstFinalistCell.Style.Font.Bold = true;
            }

            var secondFinalistCell = sheet.Cells["H3"];
            secondFinalistCell.Value = session.SecondFinalist.Name;
            secondFinalistCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            secondFinalistCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            secondFinalistCell.Style.Font.SetFromFont(fontName, commonFontSize);
            if (session.SecondFinalist.IsWinner) {
                secondFinalistCell.Style.Font.Color.SetColor(greenColor);
                secondFinalistCell.Style.Font.Bold = true;
            }

            var finalAnswersCells = sheet.Cells[2, 9, 3, 9 + max - 1];
            finalAnswersCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            finalAnswersCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            finalAnswersCells.Style.Font.SetFromFont(fontName, commonFontSize);
            finalAnswersCells.Style.Font.Color.SetColor(greyColor);
            finalAnswersCells.Value = noAnswerSymbol;

            for (var i = 1; i <= max; i++) {
                var firstFinalistAnswer = session.FirstFinalist.Statistics.Last().FinalRoundAnswers[i - 1];
                sheet.Cells[2, 8 + i].Value = firstFinalistAnswer ? correctAnswerSymbol : wrongAnswerSymbol;
                sheet.Cells[2, 8 + i].Style.VerticalAlignment = firstFinalistAnswer ? ExcelVerticalAlignment.Top : ExcelVerticalAlignment.Center;
                sheet.Cells[2, 8 + i].Style.Font.SetFromFont(fontName, firstFinalistAnswer ? commonFontSize : crossSymbolFontSize, !firstFinalistAnswer);
                sheet.Cells[2, 8 + i].Style.Font.Color.SetColor(firstFinalistAnswer ? greenColor : redColor);

                if (session.SecondFinalist.Statistics.Last().FinalRoundAnswers.Count > i - 1) {
                    var secondFinalistAnswer = session.SecondFinalist.Statistics.Last().FinalRoundAnswers[i - 1];
                    sheet.Cells[3, 8 + i].Value = secondFinalistAnswer ? correctAnswerSymbol : wrongAnswerSymbol;
                    sheet.Cells[3, 8 + i].Style.VerticalAlignment = secondFinalistAnswer ? ExcelVerticalAlignment.Top : ExcelVerticalAlignment.Center;
                    sheet.Cells[3, 8 + i].Style.Font.SetFromFont(fontName, secondFinalistAnswer ? commonFontSize : crossSymbolFontSize, !secondFinalistAnswer);
                    sheet.Cells[3, 8 + i].Style.Font.Color.SetColor(secondFinalistAnswer ? greenColor : redColor);
                }
            }

            var generalStatisticsCells = sheet.Cells[1, 1, row, 7];
            foreach (var cell in generalStatisticsCells) {
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            generalStatisticsCells.Style.Border.BorderAround(ExcelBorderStyle.Thick);

            var finalStatisticsCells = sheet.Cells[1, 8, 3, 8 + max];
            foreach (var cell in finalStatisticsCells) {
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            finalStatisticsCells.Style.Border.BorderAround(ExcelBorderStyle.Thick);

            document.Settings.TextSettings.AutofitScaleFactor = AUTOFIT_SCALE_FACTOR;
            sheet.Cells[1, 1, row, 8 + max].AutoFitColumns();
        }
        catch (Exception e) {
            logger.Error(e, "Error while generating Excel Statistics");
        }
    }

    /// <summary>
    /// Добавляет в документ статистики по раундам
    /// </summary>
    /// <param name="session">Игровая сессия</param>
    /// <param name="document">Текущий документ</param>
    private static void AddRoundStatistics(GameSession session, ExcelPackage document) {
        logger.Info("Generating Rounds Statistics...");
        try {
            foreach (var round in session.Rounds.SkipLast(1)) {
                var sheet = document.Workbook.Worksheets.Add($"Раунд {round.Number}");
                sheet.TabColor = roundStatisticsTabColor;

                var titleCells = sheet.Cells[1, 1, 1, 5];
                titleCells.Value = $"Статистика {round.Number} раунда";
                titleCells.Merge = true;
                titleCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                titleCells.Style.Font.SetFromFont(fontName, headerFontSize, true);

                var statisticsHeaderCells = sheet.Cells[2, 1, 2, 5];
                statisticsHeaderCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                statisticsHeaderCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                statisticsHeaderCells.Style.Font.SetFromFont(fontName, headerFontSize, true);
                statisticsHeaderCells.LoadFromArrays([["Имя", "Верно", "Неверно", "Банк", "Ср. ск. ответа"]]);

                var row = 2;
                var column = 1;
                foreach (var item in round.Statistics.PlayersStatistics.Values) {
                    row++;
                    sheet.Cells[row, column].Value = item.Player.Name;
                    sheet.Cells[row, column + 1].Value = item.CorrectAnswers;
                    sheet.Cells[row, column + 2].Value = item.WrongAnswers;
                    sheet.Cells[row, column + 3].Value = item.BankedMoney;
                    sheet.Cells[row, column + 4].Value = item.AverageSpeed;
                    sheet.Cells[row, column + 4].Style.Numberformat.Format = "#,##0.00";

                    if (item.IsStrongestLink) sheet.Cells[row, 1, row, column + 4].Style.Font.Color.SetColor(greenColor);
                    else if (item.IsWeakestLink) sheet.Cells[row, 1, row, column + 4].Style.Font.Color.SetColor(redColor);
                }

                var commonStatisticsCells = sheet.Cells[3, 1, row, 5];
                commonStatisticsCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                commonStatisticsCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                commonStatisticsCells.Style.Font.SetFromFont(fontName, commonFontSize);

                var bankHeaderCell = sheet.Cells["F1"];
                bankHeaderCell.Value = "Общий банк:";
                bankHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                bankHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                bankHeaderCell.Style.Font.SetFromFont(fontName, headerFontSize, true);

                var bankValueCell = sheet.Cells["F2"];
                bankValueCell.Value = round.BankedMoney;
                bankValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                bankValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                bankValueCell.Style.Font.SetFromFont(fontName, headerFontSize, true);

                var generalStatisticsCells = sheet.Cells[1, 1, row, 5];
                foreach (var cell in generalStatisticsCells) {
                    cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                generalStatisticsCells.Style.Border.BorderAround(ExcelBorderStyle.Thick);

                var bankCells = sheet.Cells[1, 6, 2, 6];
                foreach (var cell in bankCells) {
                    cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                bankCells.Style.Border.BorderAround(ExcelBorderStyle.Thick);

                document.Settings.TextSettings.AutofitScaleFactor = AUTOFIT_SCALE_FACTOR;
                sheet.Cells[1, 1, row, 6].AutoFitColumns();
            }
        }
        catch (Exception e) {
            logger.Error(e, "Error while generating Excel Statistics");
        }
    }

    /// <summary>
    /// Добавляет в документ статистики по игрокам
    /// </summary>
    /// <param name="session">Игровая сессия</param>
    /// <param name="document">Текущий документ</param>
    private static void AddPlayerStatistics(GameSession session, ExcelPackage document) {
        logger.Info("Generating Players Statistics...");
        try {
            foreach (var player in session.AllPlayers) {
                var sheet = document.Workbook.Worksheets.Add($"{player.Number}. {player.Name}");
                sheet.TabColor = playerStatisticsTabColor;

                var titleCells = sheet.Cells[1, 1, 1, 5];
                titleCells.Value = $"Статистика игрока - {player.Name}";
                titleCells.Merge = true;
                titleCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                titleCells.Style.Font.SetFromFont(fontName, headerFontSize, true);

                var statisticsHeaderCells = sheet.Cells[2, 1, 2, 5];
                statisticsHeaderCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                statisticsHeaderCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                statisticsHeaderCells.Style.Font.SetFromFont(fontName, headerFontSize, true);
                statisticsHeaderCells.LoadFromArrays([["Раунд", "Верно", "Неверно", "Банк", "Ср. ск. ответа"]]);

                var row = 2;
                var column = 1;
                foreach (var item in player.Statistics.Where(x => x.RoundName != "Финал")) {
                    row++;
                    sheet.Cells[row, column].Value = int.Parse(item.RoundName);
                    sheet.Cells[row, column + 1].Value = item.CorrectAnswers;
                    sheet.Cells[row, column + 2].Value = item.WrongAnswers;
                    sheet.Cells[row, column + 3].Value = item.BankedMoney;
                    sheet.Cells[row, column + 4].Value = item.AverageSpeed;
                    sheet.Cells[row, column + 4].Style.Numberformat.Format = "#,##0.00";

                    if (item.IsStrongestLink) sheet.Cells[row, 1, row, column + 4].Style.Font.Color.SetColor(greenColor);
                    else if (item.IsWeakestLink) sheet.Cells[row, 1, row, column + 4].Style.Font.Color.SetColor(redColor);
                }

                var commonStatisticsCells = sheet.Cells[3, 1, row, 5];
                commonStatisticsCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                commonStatisticsCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                commonStatisticsCells.Style.Font.SetFromFont(fontName, commonFontSize);

                var generalStatisticsCells = sheet.Cells[1, 1, row, 5];
                foreach (var cell in generalStatisticsCells) {
                    cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                generalStatisticsCells.Style.Border.BorderAround(ExcelBorderStyle.Thick);

                document.Settings.TextSettings.AutofitScaleFactor = AUTOFIT_SCALE_FACTOR;
                sheet.Cells[1, 1, row, 5].AutoFitColumns();
            }
        }
        catch (Exception e) {
            logger.Error(e, "Error while generating Excel Statistics");
        }
    }
}