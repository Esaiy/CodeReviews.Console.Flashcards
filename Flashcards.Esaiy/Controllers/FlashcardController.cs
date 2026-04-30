using Flashcards.Esaiy.Dtos;
using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Helpers;
using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Flashcards.Esaiy.Services;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class FlashcardController(FlashcardRepository flashcardRepository, StackService stackService)
{
    public void ManageFlashcard()
    {
        var selectedStack = stackService.SelectStack();
        if (selectedStack is null)
        {
            AnsiConsole.MarkupLine("there is no stack");
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            FigletText title = new("Flashcard");
            AnsiConsole.Write(title);
            AnsiConsole.MarkupLine($"Currently used stack: {selectedStack.Name}");
            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<FlashcardMenu>()
                    .Title("Choose Flashcard Menu")
                    .PageSize(10)
                    .MoreChoicesText("Move Up Or Down to Choose")
                    .UseConverter((option) => Formatter.FormatEnum(option))
                    .AddChoices(Enum.GetValues<FlashcardMenu>())
                    );

            switch (choice)
            {
                case FlashcardMenu.Select_Stack:
                    selectedStack = stackService.SelectStack()!;
                    continue;
                case FlashcardMenu.Create:
                    Create(selectedStack);
                    break;
                case FlashcardMenu.Get_All:
                    GetAll(selectedStack);
                    break;
                case FlashcardMenu.Update:
                    Update(selectedStack);
                    break;
                case FlashcardMenu.Delete:
                    Delete(selectedStack);
                    break;
                case FlashcardMenu.Back:
                    return;
            }
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            _ = Console.ReadKey();
        }
    }

    public void Create(Stack stack)
    {
        var front = AnsiConsole.Ask<string>("Enter the front side of the flashcard: ");
        var back = AnsiConsole.Ask<string>("Enter the back side of the flashcard: ");

        var flashcardObj = new Flashcard(front, back, stack.Id);

        flashcardRepository.Create(flashcardObj);

        AnsiConsole.MarkupLine($"New flashcard has been added to stack \"{stack.Name}\".");
    }

    public void GetAll(Stack stack)
    {
        var flashcards = flashcardRepository.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        Table table = new();
        table.AddColumn("Id")
            .AddColumn("Front")
            .AddColumn("Back");
        foreach (var f in flashcardDtos)
        {
            _ = table.AddRow(new Text(f.Id.ToString()), new Text(f.Front), new Text(f.Back));
        }

        AnsiConsole.Write(table);
    }

    public void Update(Stack stack)
    {
        var flashcards = flashcardRepository.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        Table table = new();
        table.AddColumn("Id")
            .AddColumn("Front")
            .AddColumn("Back");

        foreach (var f in flashcardDtos)
        {
            _ = table.AddRow(new Text(f.Id.ToString()), new Text(f.Front), new Text(f.Back));
        }
        AnsiConsole.Write(table);

        var prompt = new TextPrompt<string>("Select the id of the flashcard: ");
        foreach (var f in flashcardDtos)
        {
            prompt.AddChoice(f.Id.ToString());
        }
        prompt.HideChoices();

        var result = AnsiConsole.Prompt(prompt);
        var selectedFlashcardDto = flashcardDtos.Find(x => x.Id == int.Parse(result));

        AnsiConsole.MarkupLine($"The old front side is: \"{selectedFlashcardDto!.Front}\"");
        var newFront = AnsiConsole.Ask<string>("Enter new front side: ");

        AnsiConsole.MarkupLine($"The old back side is: \"{selectedFlashcardDto!.Back}\"");
        var newBack = AnsiConsole.Ask<string>("Enter new back side: ");

        var newFlashcard = new Flashcard(selectedFlashcardDto.RealId, newFront, newBack, stack.Id);

        flashcardRepository.Update(newFlashcard);

        AnsiConsole.MarkupLine("Flashcard has been updated.");
    }

    public void Delete(Stack stack)
    {
        var flashcards = flashcardRepository.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        Table table = new();
        table.AddColumn("Id")
            .AddColumn("Front")
            .AddColumn("Back");

        foreach (var f in flashcardDtos)
        {
            _ = table.AddRow(new Text(f.Id.ToString()), new Text(f.Front), new Text(f.Back));
        }
        AnsiConsole.Write(table);

        var prompt = new TextPrompt<string>("Select the id of the flashcard: ");
        foreach (var f in flashcardDtos)
        {
            prompt.AddChoice(f.Id.ToString());
        }
        prompt.HideChoices();

        var result = AnsiConsole.Prompt(prompt);
        var selectedFlashcardDto = flashcardDtos.Find(x => x.Id == int.Parse(result));

        flashcardRepository.Delete(selectedFlashcardDto!.RealId);

        AnsiConsole.MarkupLine("Flashcard has been deleted");
    }
}
