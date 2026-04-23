// See https://aka.ms/new-console-template for more information
using Flashcards.Esaiy.Database;
using Flashcards.Esaiy.Model;
using Flashcards.Esaiy.Repository;
Console.WriteLine("Hello, World!");

var connectionString = "Server=localhost;Database=flashcard;User Id=sa;Password=P@ssword123;TrustServerCertificate=True";

var SQLServerObject = new SQLServer(connectionString);
SQLServerObject.MigrateUp();

// do crud
var stackRepo = new StackRepository(SQLServerObject);
var flashcardRepo = new FlashcardRepository(SQLServerObject);

// create
var rnd = new Random();
var newStack = new Stack($"English-{rnd.NextDouble()}");
stackRepo.Create(newStack);

//read
var stacks = stackRepo.GetAll();

foreach (var s in stacks)
{
    Console.WriteLine($"{s.Id} {s.Name}");
}

var singleStack = stackRepo.Get(stacks[0].Id);
if (singleStack == null)
{
    Console.WriteLine("no stack");
}
else
{
    Console.WriteLine($"single: {singleStack.Id} {singleStack.Name}");
}

// update
var updatedStack = new Stack($"Spanish-{rnd.NextDouble()}");
if (stackRepo.Update(0, updatedStack))
{
    Console.WriteLine("updated");
}
else
{
    Console.WriteLine("lol no");
}

// delete
stackRepo.Delete(stacks[0].Id);

// create
var newFlashcard = new Flashcard("le front", "le back", stacks[^1].Id);
for (int i = 0; i < 10; i++)
{
    var toBeInserted = new Flashcard($"{newFlashcard.Front}-{rnd.Next(1, 10)}", $"{newFlashcard.Back}-{rnd.Next(1, 10)}", newFlashcard.StackId);
    flashcardRepo.Create(toBeInserted);
}

// read
var flashcards = flashcardRepo.GetAll(stacks[^1].Id);

foreach (var f in flashcards)
{
    Console.WriteLine($"{f.Id} {f.Front} {f.Back} {f.StackId}");
}

var singleFlashcard = flashcardRepo.Get(flashcards[0].Id);
if (singleFlashcard == null)
{
    Console.WriteLine("no flashcard");
}
else
{
    Console.WriteLine($"{singleFlashcard.Id} {singleFlashcard.Front} {singleFlashcard.Back} {singleFlashcard.StackId}");
}

// update
var updatedFlashcard = new Flashcard("new front", "new back", stacks[^1].Id);
if (flashcardRepo.Update(singleFlashcard.Id, updatedFlashcard))
{
    Console.WriteLine("updated");
}
else
{
    Console.WriteLine("lol no");
}

// delete
flashcardRepo.Delete(singleFlashcard.Id);
