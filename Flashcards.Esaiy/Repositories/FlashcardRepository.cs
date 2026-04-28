using Dapper;
using Flashcards.Esaiy.Databases;
using Flashcards.Esaiy.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.Esaiy.Repositories;

public class FlashcardRepository(SQLServer db)
{
    private readonly SQLServer _db = db;

    public void Create(Flashcard flashcard)
    {
        var query = @"
            INSERT INTO flashcard(Front, Back, StackId)
            VALUES (@Front, @Back, @StackId);";

        using var connection = new SqlConnection(_db.ConnectionString);
        connection.Execute(query, flashcard);
    }

    public List<Flashcard> GetAll(int stackId)
    {
        var query = @"
            SELECT Id, Front, Back, StackId
            FROM flashcard
            WHERE StackId = @stackId
            ORDER BY Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        List<Flashcard> result = [.. connection.Query<Flashcard>(query, new { stackId })];
        return result;
    }

    public Flashcard? Get(int id)
    {
        var query = @"
            SELECT Id, Front, Back, StackId
            FROM flashcard
            WHERE Id = @id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        Flashcard? result = connection.QuerySingleOrDefault<Flashcard>(query, new { Id = id });
        return result;
    }

    public bool Update(Flashcard flashcard)
    {
        var queryUpdate = @"
            UPDATE flashcard
            SET Front = @Front,
            Back = @Back
            WHERE Id = @Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        var rows = connection.Execute(queryUpdate, new { flashcard.Front, flashcard.Back, flashcard.Id });
        return rows > 0;
    }

    public bool Delete(int id)
    {
        var queryDelete = @"
            DELETE FROM flashcard
            WHERE Id = @Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        var rows = connection.Execute(queryDelete, new { Id = id });
        return rows > 0;
    }
}
