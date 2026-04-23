namespace Flashcards.Esaiy.Model;

public class Flashcard
{
    public int Id { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public int StackId { get; set; }

    public Flashcard(int id, string front, string back, int stackId)
    {
        Id = id;
        Front = front;
        Back = back;
        StackId = stackId;
    }

    public Flashcard(string front, string back, int stackId)
    {
        Front = front;
        Back = back;
        StackId = stackId;
    }
}
