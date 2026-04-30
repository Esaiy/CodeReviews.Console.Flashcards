using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Helpers;
using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class StackController(StackRepository stackRepo)
{
    public void ManageStack()
    {
        while (true)
        {
            AnsiConsole.Clear();
            FigletText title = new("Flashcard");
            AnsiConsole.Write(title);
            StackMenu choice = AnsiConsole.Prompt(
                    new SelectionPrompt<StackMenu>()
                    .Title("Choose Stack Menu")
                    .PageSize(10)
                    .MoreChoicesText("Move Up Or Down to Choose")
                    .UseConverter((option) => Helper.FormatEnum(option))
                    .AddChoices(Enum.GetValues<StackMenu>())
                    );
            switch (choice)
            {
                case StackMenu.Create:
                    Create();
                    break;
                case StackMenu.Get_All:
                    GetAll();
                    break;
                case StackMenu.Update:
                    Update();
                    break;
                case StackMenu.Delete:
                    Delete();
                    break;
                case StackMenu.Back:
                    return;
            }
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            _ = Console.ReadKey();
        }
    }

    public void Create()
    {
        var stackName = AnsiConsole.Ask<string>("Enter new stack name (must be unique): ");
        var stackObj = new Stack(stackName);

        try
        {
            stackRepo.Create(stackObj);
        }
        catch (SqlException ex)
        {
            // the way you handle the sqlclient exception is by the ex.Number
            // maybe refactor this to another function

            // exception number for unique constraint violation
            if (ex.Number == 2627)
            {
                AnsiConsole.MarkupLine($"A stack with the name \"{stackName}\" already exists.");
                AnsiConsole.MarkupLine("The name of the stack must be unique.");
                return;
            }
            else
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        AnsiConsole.MarkupLine($"New stack: \"{stackName}\" has been added.");
    }

    public void GetAll()
    {
        var stacks = stackRepo.GetAll();

        if (stacks.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no stack.");
            return;
        }

        Table readTable = new();
        readTable.AddColumn("Name");
        foreach (var s in stacks)
        {
            _ = readTable.AddRow(new Text(s.Name));
        }

        AnsiConsole.Write(readTable);
    }

    public void Update()
    {
        var stacks = stackRepo.GetAll();

        if (stacks.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no stack.");
            return;
        }

        var updateId = AnsiConsole.Prompt(
            new SelectionPrompt<Stack>()
            .Title("Select Stack")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Name}")
            .AddChoices(stacks)
            );


        AnsiConsole.MarkupLine($"The old stack name is: \"{updateId.Name}\"");
        var stackName = AnsiConsole.Ask<string>("Enter the new stack name: ");
        updateId.Name = stackName;
        stackRepo.Update(updateId);

        AnsiConsole.MarkupLine("Stack successfully updated.");
    }

    public void Delete()
    {
        var stacks = stackRepo.GetAll();

        if (stacks.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no stack.");
            return;
        }

        var deletedStack = AnsiConsole.Prompt(
            new SelectionPrompt<Stack>()
            .Title("Select Stack")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Name}")
            .AddChoices(stacks)
            );

        bool confirmation = AnsiConsole.Prompt(
            new TextPrompt<bool>("Confirm?")
            .AddChoice(true)
            .AddChoice(false)
            .DefaultValue(true)
            .WithConverter(static choice => choice ? "y" : "n")
            );

        if (!confirmation)
        {
            AnsiConsole.MarkupLine("Canceled");
            return;
        }

        stackRepo.Delete(deletedStack.Id);
        AnsiConsole.MarkupLine("Stack has been deleted.");
    }
}
