namespace Flashcards.Esaiy.Helpers;

public class Helper
{

    public static string FormatEnum(Enum e)
    {
        return e.ToString().Replace("_", " ");
    }
}
