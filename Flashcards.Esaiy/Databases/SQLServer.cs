using Dapper;
using Microsoft.Data.SqlClient;

namespace Flashcards.Esaiy.Databases;

public class SqlServer(string connString)
{
    public string ConnectionString { get; } = connString;

    public void MigrateUp()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            var stackQuery = @"
                    IF OBJECT_ID('dbo.stack', 'U') IS NULL
                    BEGIN
                      CREATE TABLE stack
                      (
                        Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
                        Name NVARCHAR(100) UNIQUE NOT NULL
                      );
                    END";
            connection.Execute(stackQuery);

            var flashcardQuery = @"
                    IF OBJECT_ID('dbo.flashcard', 'U') IS NULL
                    BEGIN
                      CREATE TABLE flashcard
                      (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Front NVARCHAR(255) NOT NULL,
                        Back NVARCHAR(255) NOT NULL,
                        StackId INT NOT NULL,
                        CONSTRAINT FK_flashcard_stack FOREIGN KEY (StackId)
                        REFERENCES stack (Id)
                        ON DELETE CASCADE
                      );
                    END";
            connection.Execute(flashcardQuery);

            var studySessionQuery = @"
                    IF OBJECT_ID('dbo.study_session', 'U') IS NULL
                    BEGIN
                      CREATE TABLE study_session
                      (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        CorrectAnswer INT NOT NULL,
                        TotalQuestions INT NOT NULL,
                        Date DATETIME NOT NULL,
                        StackId INT NOT NULL,
                        CONSTRAINT FK_study_session_stack FOREIGN KEY (StackId)
                        REFERENCES stack (Id)
                        ON DELETE CASCADE
                      );
                    END";
            connection.Execute(studySessionQuery);
        }
        catch
        {
            Console.WriteLine("Failed creating initial databases");
            throw;
        }
    }

    public void MigrateDown()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            var dropQuery = @"
                    DROP TABLE IF EXISTS flashcard;
                    DROP TABLE IF EXISTS stack;
                    DROP TABLE IF EXISTS study_session";
            connection.Execute(dropQuery);
        }
        catch
        {
            Console.WriteLine("Failed deleting databases");
            throw;
        }
    }
}


