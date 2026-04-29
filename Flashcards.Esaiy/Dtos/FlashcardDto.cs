using Flashcards.Esaiy.Models;

namespace Flashcards.Esaiy.Dtos;

public class FlashcardDto
{
    public int Id { get; set; }
    public int RealId { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;

    public static List<FlashcardDto> ModelToDto(List<Flashcard> flashcardModels)
    {
        List<FlashcardDto> dtos = [];
        var i = 1;
        foreach (var f in flashcardModels)
        {
            dtos.Add(new FlashcardDto
            {
                Id = i++,
                RealId = f.Id,
                Front = f.Front,
                Back = f.Back
            });
        }
        return dtos;
    }
}
