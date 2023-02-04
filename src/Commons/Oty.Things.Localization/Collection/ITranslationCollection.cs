namespace Oty.Things.Localization;

/// <summary>
/// Represents collection getter for all specifed cultural translations.
/// </summary>
[PublicAPI]
public interface ITranslationCollection
{
    /// <summary>
    /// Gets the translations with specified id.
    /// </summary>
    /// <param name="id">Id of translation</param>
    /// <returns>All of the translation for the specified id</returns>
    [PublicAPI]
    IReadOnlyDictionary<string, string?>? this[string id] { get; }

    /// <summary>
    /// Gets the translation with specified values.
    /// </summary>
    /// <param name="id">Id or text context of translation.</param>
    /// <param name="arguments">Arguments for the string format with text context, translation finding style and its plural as well.</param>
    /// <returns>All of the translation for the specified id</returns>
    [PublicAPI]
    IReadOnlyDictionary<string, string?>? this[string id, params object[] arguments] { get; }

    /// <summary>
    /// Gets the all cultural translations from specified values.
    /// </summary>
    /// <param name="id">Id of the translation.</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <returns>All of the translation for the specified id</returns>
    [PublicAPI]
    IReadOnlyDictionary<string, string?>? GetAllTranslations(string id, Plural plural, TextContext textContext, params object[]? arguments);

    /// <summary>
    /// Gets the all cultural translations from specified values.
    /// </summary>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <returns>All of the translation for the specified id</returns>
    [PublicAPI]
    IReadOnlyDictionary<string, string?>? GetAllTranslations(string textContext, Plural plural, params object[]? arguments);
}