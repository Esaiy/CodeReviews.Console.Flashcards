using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Model;
using Flashcards.Esaiy.Repository;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards.Esaiy.Controller;

public class StackController(StackRepository stackRepo)
{
    public void ManageStack()
    {
        while (true)
        {
            AnsiConsole.Clear();
            FigletText title = new("Flashcard");
            AnsiConsole.Write(title);
            Enums.StackMenu choice = AnsiConsole.Prompt(
                    new SelectionPrompt<Enums.StackMenu>()
                    .Title("Choose Stack Menu")
                    .PageSize(10)
                    .MoreChoicesText("Move Up Or Down to Choose")
                    .AddChoices(Enum.GetValues<Enums.StackMenu>())
                    );
            switch (choice)
            {
                case Enums.StackMenu.Create:
                    Create();
                    break;
                case Enums.StackMenu.GetAll:
                    GetAll();
                    break;
                case Enums.StackMenu.Pick:
                    break;
                case Enums.StackMenu.Update:
                    Update();
                    break;
                case Enums.StackMenu.Delete:
                    Delete();
                    break;
                case Enums.StackMenu.Back:
                    return;
            }
            AnsiConsole.MarkupLine("Press Any Key to Continue");
            _ = Console.ReadKey();
        }
    }

    public void Create()
    {
        var stackName = AnsiConsole.Ask<string>("enter stack name");
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
        var listStack = stackRepo.GetAll();

        if (listStack.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no stack.");
            return;
        }

        Table readTable = new();
        readTable.AddColumn("Id")
            .AddColumn("Name");
        foreach (var s in listStack)
        {
            _ = readTable.AddRow(new Text(s.Id.ToString()), new Text(s.Name));
        }

        AnsiConsole.Write(readTable);
    }

    public void Pick()
    {

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
            .Title("Select Coding Session")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Id} {s.Name}")
            .AddChoices(stacks)
            );


        var stackName = AnsiConsole.Ask<string>("enter new stack name");
        updateId.Name = stackName;
        stackRepo.Update(updateId);

        AnsiConsole.MarkupLine("its updated");
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
            .Title("Select Coding Session")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Id} {s.Name}")
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
            AnsiConsole.MarkupLine("cancel deletion");
            return;
        }

        stackRepo.Delete(deletedStack.Id);
        AnsiConsole.MarkupLine("its deleted");
    }
}
