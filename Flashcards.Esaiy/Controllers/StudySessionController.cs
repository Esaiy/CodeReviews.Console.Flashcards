using Flashcards.Esaiy.Dtos;
using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Helpers;
using Flashcards.Esaiy.Models;
using Flashcards.Esaiy.Repositories;
using Flashcards.Esaiy.Services;
using Spectre.Console;

namespace Flashcards.Esaiy.Controllers;

public class StudySessionController(StudySessionRepository studySessionRepository, FlashcardRepository flashcardRepository, StackService stackService)
{
    public void Study()
    {
        var selectedStack = stackService.SelectStack();
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
                    .UseConverter((option) => Helper.FormatEnum(option))
                    .AddChoices(Enum.GetValues<StudyMenu>())
                    );
            switch (choice)
            {
                case StudyMenu.Select_Stack:
                    selectedStack = stackService.SelectStack()!;
                    continue;
                case StudyMenu.Start:
                    Start(selectedStack);
                    break;
                case StudyMenu.Get_All_Sessions:
                    GetAll(selectedStack);
                    break;
                case StudyMenu.Back:
                    return;
            }
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            _ = Console.ReadKey();
        }
    }

    public void Start(Stack stack)
    {
        var flashcards = flashcardRepository.GetAll(stack.Id);
        if (flashcards.Count == 0)
        {
            AnsiConsole.MarkupLine("There is no flashcard in this stack.");
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

            var back = AnsiConsole.Ask<string>("What is the answer for this card?");
            if (f.Back.Equals(back, StringComparison.OrdinalIgnoreCase))
            {
                correctAnswer++;
                AnsiConsole.MarkupLine($"Thats right, \"{back}\" is the correct answer.");
            }
            else
            {
                AnsiConsole.MarkupLine($"It's wrong, the right answer is \"{f.Back}\"");
            }

            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            _ = Console.ReadKey();
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Flashcard"));

        var obj = new StudySession(correctAnswer, flashcards.Count, startTime, stack.Id);
        studySessionRepository.Save(obj);

        var score = (double)correctAnswer / flashcards.Count * 100;
        AnsiConsole.MarkupLine($"finished a session with a score: {score} ({correctAnswer}/{flashcards.Count})");
    }

    public void GetAll(Stack stack)
    {
        var studySessions = studySessionRepository.GetAll(stack.Id);

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
            .AddColumn("Score")
            .AddColumn("Date");
        foreach (var l in studySessionDtos)
        {
            _ = readTable.AddRow(new Text(l.Id.ToString()), new Text(l.CorrectAnswer.ToString()), new Text(l.TotalQuestions.ToString()), new Text(l.Score.ToString()), new Text(l.Date.ToString()));
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
