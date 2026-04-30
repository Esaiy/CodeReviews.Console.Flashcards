namespace Flashcards.Esaiy.Models;

public class Stack
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Stack(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Stack(string name)
    {
        Name = name;
    }
}
