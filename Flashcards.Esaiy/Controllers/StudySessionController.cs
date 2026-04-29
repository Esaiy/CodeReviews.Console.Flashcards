using Flashcards.Esaiy.Dtos;
using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class StudySessionController(StudySessionRepository studySessionRepo, FlashcardRepository flashcardRepo, StackRepository stackRepo)
{
    public void Study()
    {
        var selectedStack = SelectStack();
        if (selectedStack is null)
        {
            AnsiConsole.MarkupLine("There is no stack");
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            FigletText title = new("Flashcard");
            AnsiConsole.Write(title);
            StudyMenu choice = AnsiConsole.Prompt(
                    new SelectionPrompt<StudyMenu>()
                    .Title("Choose Study Menu")
                    .PageSize(10)
                    .MoreChoicesText("Move Up Or Down to Choose")
                    .AddChoices(Enum.GetValues<StudyMenu>())
                    );
            switch (choice)
            {
                case StudyMenu.SelectStack:
                    selectedStack = SelectStack()!;
                    continue;
                case StudyMenu.Start:
                    Start(selectedStack);
                    break;
                case StudyMenu.GetAll:
                    GetAll(selectedStack);
                    break;
                case StudyMenu.Back:
                    return;
            }
            AnsiConsole.MarkupLine("Press Any Key to Continue");
            _ = Console.ReadKey();
        }
    }

    public Stack? SelectStack()
    {
        var stacks = stackRepo.GetAll();

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

    public void Start(Stack stack)
    {
        var flashcards = flashcardRepo.GetAll(stack.Id);
        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("");
            return;
        }

        var startTime = DateTime.Now;

        FisherYatesShuffle(flashcards);

        var correctAnswer = 0;
        foreach (var f in flashcards)
        {
            AnsiConsole.Clear();
            FigletText title = new("Flashcard");
            AnsiConsole.Write(title);

            Table readTable = new();
            readTable.AddColumn("Front");
            _ = readTable.AddRow(new Text(f.Front));
            AnsiConsole.Write(readTable);

            var back = AnsiConsole.Ask<string>("whats on the back side?");
            if (f.Back.Equals(back, StringComparison.OrdinalIgnoreCase))
            {
                correctAnswer++;
                AnsiConsole.MarkupLine($"thats right, \"{back}\" is the correct answer");
            }
            else
            {
                AnsiConsole.MarkupLine($"no its wrong, the right answer is \"{f.Back}\"");
            }

            AnsiConsole.MarkupLine("Press Any Key to Continue");
            _ = Console.ReadKey();
        }

        var obj = new StudySession(correctAnswer, flashcards.Count, startTime, stack.Id);
        studySessionRepo.Save(obj);

        AnsiConsole.MarkupLine($"finished a session with a score: {correctAnswer}/{flashcards.Count}");
    }

    public void GetAll(Stack stack)
    {
        var studySessions = studySessionRepo.GetAll(stack.Id);

        if (studySessions.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
            return;
        }

        var studySessionDtos = StudySessionDto.ModelToDTO(studySessions);

        Table readTable = new();
        readTable.AddColumn("Id")
            .AddColumn("Correct Answer")
            .AddColumn("Total Questions")
            .AddColumn("Date");
        foreach (var l in studySessionDtos)
        {
            _ = readTable.AddRow(new Text(l.Id.ToString()), new Text(l.CorrectAnswer.ToString()), new Text(l.TotalQuestions.ToString()), new Text(l.Date.ToString()));
        }

        AnsiConsole.Write(readTable);
    }

    public static void FisherYatesShuffle<T>(IList<T> list)
    {
        Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
