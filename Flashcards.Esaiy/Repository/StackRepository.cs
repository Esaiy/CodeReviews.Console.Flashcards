using Dapper;
using Flashcards.Esaiy.Database;
using Flashcards.Esaiy.Model;
using Microsoft.Data.SqlClient;

namespace Flashcards.Esaiy.Repository;

public class StackRepository(SQLServer db)
{
    private readonly SQLServer _db = db;

    public void Create(Stack stack)
    {
        var query = @"
            INSERT INTO stack(Name)
            VALUES (@Name);";

        using var connection = new SqlConnection(_db.ConnectionString);
        connection.Execute(query, stack);
    }

    public List<Stack> GetAll()
    {
        var query = @"
            SELECT Id, Name
            FROM stack
            ORDER BY Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        List<Stack> result = [.. connection.Query<Stack>(query)];
        return result;
    }

    public Stack? Get(int id)
    {
        var query = @"
            SELECT Id, Name
            FROM stack
            WHERE Id = @id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        Stack? result = connection.QuerySingleOrDefault<Stack>(query, new { Id = id });
        return result;
    }

    public bool Update(int id, Stack stack)
    {
        var queryUpdate = @"
            UPDATE stack
            SET Name = @Name
            WHERE Id = @Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        var rows = connection.Execute(queryUpdate, new { stack.Name, Id = id });
        return rows > 0;
    }

    public bool Delete(int id)
    {
        var queryDelete = @"
            DELETE FROM stack
            WHERE Id = @Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        var rows = connection.Execute(queryDelete, new { Id = id });
        return rows > 0;
    }
}
