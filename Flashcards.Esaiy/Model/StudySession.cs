namespace Flashcards.Esaiy.Model;

public class StudySession
{
    public int Id { get; set; }
    public int CorrectAnswer { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime Date { get; set; }
    public int StackId { get; set; }

    public StudySession(int id, int correctAnswer, int totalQuestions, DateTime date, int stackId)
    {
        Id = id;
        CorrectAnswer = correctAnswer;
        TotalQuestions = totalQuestions;
        Date = date;
        StackId = stackId;
    }

    public StudySession(int correctAnswer, int totalQuestions, DateTime date, int stackId)
    {
        CorrectAnswer = correctAnswer;
        TotalQuestions = totalQuestions;
        Date = date;
        StackId = stackId;
    }
}
