// See https://aka.ms/new-console-template for more information
using Flashcards.Esaiy.Database;
Console.WriteLine("Hello, World!");

var connectionString = "Server=localhost;Database=flashcard;User Id=sa;Password=P@ssword123;TrustServerCertificate=True";

var SQLServerObject = new SQLServer(connectionString);
SQLServerObject.MigrateUp();
