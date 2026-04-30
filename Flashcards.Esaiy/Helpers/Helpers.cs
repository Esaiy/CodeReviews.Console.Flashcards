namespace Flashcards.Esaiy.Helpers;

public class Formatter
{

    public static string FormatEnum(Enum e)
    {
        return e.ToString().Replace("_", " ");
    }
}
