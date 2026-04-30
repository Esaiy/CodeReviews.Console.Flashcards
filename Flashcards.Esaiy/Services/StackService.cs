using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Spectre.Console;

namespace Flashcards.Esaiy.Services;

public class StackService(StackRepository stackRepository)
{
    public Stack? SelectStack()
    {
        var stacks = stackRepository.GetAll();

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
}
