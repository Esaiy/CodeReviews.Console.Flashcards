namespace Flashcards.Esaiy.Model;

public class Stack
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Stack(int id, string name)
    {
        // TODO: handle empty string for name
        Id = id;
        Name = name;
    }

    public Stack(string name)
    {
        // TODO: handle empty string for name
        Name = name;
    }
}
