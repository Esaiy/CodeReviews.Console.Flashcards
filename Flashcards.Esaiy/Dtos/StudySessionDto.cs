using Flashcards.Esaiy.Models;

namespace Flashcards.Esaiy.Dtos;

public class StudySessionDTO
{
    public int Id { get; set; }
    public int CorrectAnswer { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime Date { get; set; }

    public static List<StudySessionDTO> ModelToDTO(List<StudySession> studySessionModels)
    {
        List<StudySessionDTO> dtos = [];
        var i = 1;
        foreach (var s in studySessionModels)
        {
            dtos.Add(new StudySessionDTO
            {
                Id = i++,
                CorrectAnswer = s.CorrectAnswer,
                TotalQuestions = s.TotalQuestions,
                Date = s.Date,
            });
        }
        return dtos;
    }
}
