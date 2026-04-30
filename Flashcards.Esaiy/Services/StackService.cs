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

        Table table = new();
        table.AddColumn("Name");
        foreach (var s in stacks)
        {
            _ = table.AddRow(new Text(s.Name));
        }
        AnsiConsole.Write(table);

        var prompt = new TextPrompt<string>("Select the stack: ");
        foreach (var stack in stacks)
        {
            prompt.AddChoice(stack.Name);
        }
        prompt.HideChoices();

        var result = AnsiConsole.Prompt(prompt);
        var selectedStack = stacks.Find(x => x.Name == result);

        return selectedStack;
    }
}
