namespace Oty.CommandLib.Interactions;

public interface ILocalizationProvider
{
    IReadOnlyDictionary<string, LocalizedValues> GetLocalizations();
}