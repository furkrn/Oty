namespace Oty.Things.Localization;

internal static class ObjectArrayExtensions
{
    internal static object[]? GetSpecialArguments(this object[] arguments, out TranslationFindingType type, out Plural? plural, out TextContext? textContext)
    {
        int startIndex = 0;

        var findingType = arguments.ElementAtOrDefault(startIndex) as TranslationFindingType?;

        if (findingType.HasValue)
        {
            startIndex++;
        }

        type = findingType ?? TranslationFindingType.WithSpecifyingEverything;
        plural = null;
        textContext = null;

        if (arguments.ElementAtOrDefault(startIndex) is Plural p)
        {
            startIndex++;
            plural = p;
        }

        if (type is TranslationFindingType.WithSpecifyingEverything && 
            arguments.ElementAtOrDefault(startIndex) is TextContext tx)
        {
            startIndex++;
            textContext = tx;
        }

        return arguments.Length > startIndex
            ? arguments[startIndex..]
            : null;
    }
}