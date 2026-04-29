using Flashcards.Esaiy.Models;

namespace Flashcards.Esaiy.Dtos;

public class StudySessionDto
{
    public int Id { get; set; }
    public int CorrectAnswer { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime Date { get; set; }
    public double Score => (double)CorrectAnswer / TotalQuestions * 100;

    public static List<StudySessionDto> ModelToDTO(List<StudySession> studySessionModels)
    {
        List<StudySessionDto> dtos = [];
        var i = 1;
        foreach (var s in studySessionModels)
        {
            dtos.Add(new StudySessionDto
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
