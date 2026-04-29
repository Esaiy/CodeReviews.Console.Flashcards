using Dapper;
using Flashcards.Esaiy.Databases;
using Flashcards.Esaiy.Dtos;
using Flashcards.Esaiy.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.Esaiy.Repositories;

public class ReportRepository(SqlServer db)
{
    private readonly SqlServer _db = db;

    public ReportDto? GetReport(Stack stack, int year)
    {
        var query = @"
            SELECT
                Name as StackName,
                ISNULL([January], 0) AS January,
                ISNULL([February], 0) AS February,
                ISNULL([March], 0) AS March,
                ISNULL([April], 0) AS April,
                ISNULL([May], 0) AS May,
                ISNULL([June], 0) AS June,
                ISNULL([July], 0) AS July,
                ISNULL([August], 0) AS August,
                ISNULL([September], 0) AS September,
                ISNULL([October], 0) AS October,
                ISNULL([November], 0) AS November,
                ISNULL([December], 0) AS December
            FROM
            (
                SELECT
                    DATENAME(MONTH, Date) AS MonthName,
                    CAST(CorrectAnswer AS FLOAT) / TotalQuestions * 100 AS Score,
                    YEAR(date) AS [year],
                    stack.Name
                FROM study_session
                JOIN stack ON study_session.StackId = stack.id
                WHERE YEAR(date) = @Year
                AND stack.id = @Id
            ) AS src
            PIVOT
            (
                AVG(Score)
                FOR MonthName IN (
                    [January],[February],[March],[April],
                    [May],[June],[July],[August],
                    [September],[October],[November],[December]
                )
            ) AS pvt;";

        using var connection = new SqlConnection(_db.ConnectionString);

        var result = connection.QuerySingleOrDefault<ReportDto?>(query, new { stack.Id, Year = year });

        return result;
    }

    public List<int> GetYears(Stack stack)
    {
        var query = @"
            SELECT DISTINCT(year(date)) AS Year
            FROM study_session
            WHERE StackId = @Id
            ORDER BY Year ASC;";

        using var connection = new SqlConnection(_db.ConnectionString);

        List<int> result = [.. connection.Query<int>(query, new { stack.Id })];

        return result;
    }
}

