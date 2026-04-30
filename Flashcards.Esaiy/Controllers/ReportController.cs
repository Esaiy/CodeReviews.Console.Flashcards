using Flashcards.Esaiy.Repositories;
using Spectre.Console;
using Flashcards.Esaiy.Services;

namespace Flashcards.Esaiy.Controllers;

public class ReportController(ReportRepository reportRepository, StackService stackService)
{
    public void GetReport()
    {
        var selectedStack = stackService.SelectStack();
        if (selectedStack is null)
        {
            AnsiConsole.MarkupLine("There is no stack.");
            return;
        }

        var years = reportRepository.GetYears(selectedStack);

        if (years.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no recorded study session.");
            return;
        }

        var selectedYear = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
            .Title("Select Year")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .AddChoices(years)
            );

        var report = reportRepository.GetReport(selectedStack, selectedYear);

        if (report is null)
        {
            AnsiConsole.MarkupLine("There is no report for this year");
            return;
        }

        Table readTable = new();
        readTable.AddColumn("Stack Name")
            .AddColumn("January")
            .AddColumn("February")
            .AddColumn("March")
            .AddColumn("April")
            .AddColumn("May")
            .AddColumn("June")
            .AddColumn("July")
            .AddColumn("August")
            .AddColumn("September")
            .AddColumn("October")
            .AddColumn("November")
            .AddColumn("December");

        readTable.AddRow(
            new Text(report.StackName),
            new Text(report.January.ToString()),
            new Text(report.February.ToString()),
            new Text(report.March.ToString()),
            new Text(report.April.ToString()),
            new Text(report.May.ToString()),
            new Text(report.June.ToString()),
            new Text(report.July.ToString()),
            new Text(report.August.ToString()),
            new Text(report.September.ToString()),
            new Text(report.October.ToString()),
            new Text(report.November.ToString()),
            new Text(report.December.ToString())
           );

        AnsiConsole.Write(readTable);

        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        _ = Console.ReadKey();
    }
}
