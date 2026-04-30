// See https://aka.ms/new-console-template for more information
using Flashcards.Esaiy.Controllers;
using Flashcards.Esaiy.Databases;
using Flashcards.Esaiy.Repositories;
using Spectre.Console;
using Flashcards.Esaiy.Enums;
using Flashcards.Esaiy.Helpers;
using Flashcards.Esaiy.Services;

var connectionString = "Server=localhost;Database=flashcard;User Id=sa;Password=P@ssword123;TrustServerCertificate=True";

var SQLServerObject = new SqlServer(connectionString);
SQLServerObject.MigrateUp();

var stackRepository = new StackRepository(SQLServerObject);
var stackService = new StackService(stackRepository);
var stackController = new StackController(stackRepository);

var flashcardRepository = new FlashcardRepository(SQLServerObject);
var flashcardController = new FlashcardController(flashcardRepository, stackService);

var studySessionRepository = new StudySessionRepository(SQLServerObject);
var studySessionController = new StudySessionController(studySessionRepository, flashcardRepository, stackService);

var reportRepository = new ReportRepository(SQLServerObject);
var reportController = new ReportController(reportRepository, stackService);

while (true)
{
    AnsiConsole.Clear();
    FigletText title = new("Flashcard");
    AnsiConsole.Write(title);
    Main choice = AnsiConsole.Prompt(
            new SelectionPrompt<Main>()
            .Title("Choose Menu")
            .PageSize(10)
            .MoreChoicesText("Move Up Or Down to Choose")
            .UseConverter((option) => Formatter.FormatEnum(option))
            .AddChoices(Enum.GetValues<Main>())
            );

    AnsiConsole.WriteLine($"{choice}");

    switch (choice)
    {
        case Main.Manage_Stack:
            stackController.ManageStack();
            break;
        case Main.Manage_Flashcard:
            flashcardController.ManageFlashcard();
            break;
        case Main.Study:
            studySessionController.Study();
            break;
        case Main.View_Report:
            reportController.GetReport();
            break;
        case Main.Exit:
            AnsiConsole.WriteLine("Exiting Program");
            return;
        default:
            AnsiConsole.WriteLine("invalid");
            break;
    }
}


