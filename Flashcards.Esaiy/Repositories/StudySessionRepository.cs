using Dapper;
using Flashcards.Esaiy.Databases;
using Flashcards.Esaiy.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.Esaiy.Repositories;

public class StudySessionRepository(SqlServer db)
{
    private readonly SqlServer _db = db;

    public void Save(StudySession studySession)
    {
        var query = @"
            INSERT INTO study_session(CorrectAnswer, TotalQuestions, Date, StackId)
            VALUES (@CorrectAnswer, @TotalQuestions, @Date, @StackId);";

        using var connection = new SqlConnection(_db.ConnectionString);
        connection.Execute(query, studySession);
    }

    public List<StudySession> GetAll(int stackId)
    {
        var query = @"
            SELECT Id, CorrectAnswer, TotalQuestions, Date, StackId
            FROM study_session
            WHERE StackId = @Id;";

        using var connection = new SqlConnection(_db.ConnectionString);
        List<StudySession> result = [.. connection.Query<StudySession>(query, new { Id = stackId })];
        return result;
    }
};
