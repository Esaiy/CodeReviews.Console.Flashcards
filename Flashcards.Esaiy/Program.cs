// See https://aka.ms/new-console-template for more information
using Flashcards.Esaiy.Controller;
using Flashcards.Esaiy.Database;
using Flashcards.Esaiy.Repository;
using Spectre.Console;
using Flashcards.Esaiy.Enums;

Console.WriteLine("Hello, World!");

var connectionString = "Server=localhost;Database=flashcard;User Id=sa;Password=P@ssword123;TrustServerCertificate=True";

var SQLServerObject = new SQLServer(connectionString);
SQLServerObject.MigrateUp();

var stackRepository = new StackRepository(SQLServerObject);
var stackController = new StackController(stackRepository);

var flashcardRepository = new FlashcardRepository(SQLServerObject);
var flashcardController = new FlashcardController(flashcardRepository, stackRepository);

var studySessionRepository = new StudySessionRepository(SQLServerObject);
var studySessionController = new StudySessionController(studySessionRepository, flashcardRepository, stackRepository);

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
            .AddChoices(Enum.GetValues<Main>())
            );

    AnsiConsole.WriteLine($"{choice}");

    switch (choice)
    {
        case Main.ManageStack:
            stackController.ManageStack();
            break;
        case Main.ManageFlashcard:
            flashcardController.ManageFlashcard();
            break;
        case Main.Study:
            studySessionController.Study();
            break;
        case Main.ViewReport:
            break;
        case Main.Exit:
            AnsiConsole.WriteLine("Exiting Program");
            return;
        default:
            AnsiConsole.WriteLine("invalid");
            break;
    }
}


