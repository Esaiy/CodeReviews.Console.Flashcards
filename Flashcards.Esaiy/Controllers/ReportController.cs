using Flashcards.Esaiy.Repositories;
using Flashcards.Esaiy.Models;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class ReportController(ReportRepository reportRepository, StackRepository stackRepository)
{
    public void GetReport()
    {
        var selectedStack = SelectStack();
        if (selectedStack is null)
        {
            // no stack
            return;
        }

        var years = reportRepository.GetYears(selectedStack);

        if (years.Count == 0)
        {
            // no study session to report
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
            // empty
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

        AnsiConsole.MarkupLine("Press Any Key to Continue");
        _ = Console.ReadKey();
    }

    public Stack? SelectStack()
    {
        var stacks = stackRepository.GetAll();

        if (stacks.Count == 0)
        {
            return null;
        }

        var selectedStack = AnsiConsole.Prompt(
            new SelectionPrompt<Stack>()
            .Title("Select Stack")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Id} {s.Name}")
            .AddChoices(stacks)
            );

        return selectedStack;
    }
}
