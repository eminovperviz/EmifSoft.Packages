namespace EmiSoft.Repository.EntityFrameworkCore.Utility;

public class LanguageOperation
{
    private LanguageOperation(string culture, int value) { Culture = culture; Value = value; }

    public string Culture { get; private set; }
    public int Value { get; private set; }

    public static LanguageOperation Azerbaijan { get { return new LanguageOperation("az-AZ", (int)LanguageCode.Azerbaijan); } }
    public static LanguageOperation English { get { return new LanguageOperation("en-US", (int)LanguageCode.English); } }
    public static LanguageOperation Russian { get { return new LanguageOperation("ru-RU", (int)LanguageCode.Russian); } }

    public static LanguageOperation Find(string clture)
    {
        if (clture == Azerbaijan.Culture)
            return Azerbaijan;
        else if (clture == English.Culture)
            return English;
        else if (clture == Russian.Culture)
            return Russian;

        return English;
    }

    public static LanguageOperation Find(int languageId)
    {
        if (languageId == (int)LanguageCode.Azerbaijan)
            return Azerbaijan;
        else if (languageId == (int)LanguageCode.English)
            return English;
        else if (languageId == (int)LanguageCode.Russian)
            return Russian;

        return English;
    }
}

public enum LanguageCode
{
    None,
    Azerbaijan = 1,
    English = 2,
    Russian = 3
}