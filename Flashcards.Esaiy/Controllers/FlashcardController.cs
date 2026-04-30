using Flashcards.Esaiy.Dtos;
using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Helpers;
using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class FlashcardController(FlashcardRepository flashcardRepo, StackRepository stackRepo)
{
    public void ManageFlashcard()
    {
        var selectedStack = SelectStack();
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
                    .UseConverter((option) => Helper.FormatEnum(option))
                    .AddChoices(Enum.GetValues<FlashcardMenu>())
                    );

            switch (choice)
            {
                case FlashcardMenu.Select_Stack:
                    selectedStack = SelectStack()!;
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

    // TODO: maybe refactor this into stack related class
    public Stack? SelectStack()
    {
        var stacks = stackRepo.GetAll();

        if (stacks.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no stack.");
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

    public void Create(Stack stack)
    {
        var front = AnsiConsole.Ask<string>("Enter the front side of the flashcard");
        var back = AnsiConsole.Ask<string>("Enter the back side of the flashcard");

        var flashcardObj = new Flashcard(front, back, stack.Id);

        flashcardRepo.Create(flashcardObj);

        AnsiConsole.MarkupLine($"New flashcard has been added to stack \"{stack.Name}\".");
    }

    public void GetAll(Stack stack)
    {
        var flashcards = flashcardRepo.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        Table readTable = new();
        readTable.AddColumn("Id")
            .AddColumn("Front")
            .AddColumn("Back");
        foreach (var f in flashcardDtos)
        {
            _ = readTable.AddRow(new Text(f.Id.ToString()), new Text(f.Front), new Text(f.Back));
        }

        AnsiConsole.Write(readTable);
    }

    public void Update(Stack stack)
    {
        var flashcards = flashcardRepo.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        var updateId = AnsiConsole.Prompt(
            new SelectionPrompt<FlashcardDto>()
            .Title("Select Flashcard")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Id} {s.Front} {s.Back}")
            .AddChoices(flashcardDtos)
            );

        var newFront = AnsiConsole.Ask<string>("Enter new front side");
        var newBack = AnsiConsole.Ask<string>("Enter new back side");

        var newFlashcard = new Flashcard(updateId.RealId, newFront, newBack, stack.Id);

        flashcardRepo.Update(newFlashcard);

        AnsiConsole.MarkupLine("Flashcard has been updated.");
    }

    public void Delete(Stack stack)
    {
        var flashcards = flashcardRepo.GetAll(stack.Id);

        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var flashcardDtos = FlashcardDto.ModelToDto(flashcards);

        var deletedFlashcard = AnsiConsole.Prompt(
            new SelectionPrompt<FlashcardDto>()
            .Title("Select Flashcard")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter(s => $"{s.Id} {s.Front} {s.Back}")
            .AddChoices(flashcardDtos)
            );

        flashcardRepo.Delete(deletedFlashcard.RealId);

        AnsiConsole.MarkupLine("Flashcard has been deleted");
    }
}
