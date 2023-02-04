namespace Oty.Things.Localization;

/// <summary>
/// Represents a service that contains PO based localization.
/// </summary>
[PublicAPI]
public interface IPoStringLocalizer : IStringLocalizer
{
    /// <summary>
    /// Gets the translation from specified values
    /// </summary>
    /// <param name="id">Id of the translation.</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <returns></returns>
    [PublicAPI]
    LocalizedString GetTranslation(string id, Plural plural, TextContext textContext, params object[]? arguments);

    /// <summary>
    /// Gets the translation from specified values
    /// </summary>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <returns></returns>
    [PublicAPI]
    LocalizedString GetTranslation(string textContext, Plural plural, params object[]? arguments);

    /// <summary>
    /// Attempts to get translation with specified values.
    /// </summary>
    /// <param name="id">Id of the translation</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="translation">The translated value. Not <see langword="null"/> if returned <see langword="true"/></param>
    /// <returns>If a attempted translation found returns <see langword="true"/> with specified translation using <paramref name="translation"/>, otherwise returned <see langword="false"/> with <see langword="null"/> to the parameter.</returns>
    [PublicAPI]
    bool TryGetTranslation(string id, Plural plural, TextContext textContext, [MaybeNullWhen(false)] out string? translation);
    
    /// <summary>
    /// Attempts to get translation with specified values.
    /// </summary>
    /// <param name="id">Id of the translation</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <param name="translation">The translated value. Not <see langword="null"/> if returned <see langword="true"/></param>
    /// <returns>If a attempted translation found returns <see langword="true"/> with specified translation using <paramref name="translation"/>, otherwise returned <see langword="false"/> with <see langword="null"/> to the parameter.</returns>
    [PublicAPI]
    bool TryGetTranslation(string id, Plural plural, TextContext textContext, object[]? arguments, [MaybeNullWhen(false)] out string? translation);

    /// <summary>
    /// Attempts to get translation with specified values.
    /// </summary>
    /// <param name="textContext">Text context of the translation beside from its id.</param>
    /// <param name="plural">Count and plural id of the translation</param>
    /// <param name="arguments">Optional arguments required for formatting the string.</param>
    /// <param name="translation">The translated value. Not <see langword="null"/> if returned <see langword="true"/></param>
    /// <returns>If a attempted translation found returns <see langword="true"/> with specified translation using <paramref name="translation"/>, otherwise returned <see langword="false"/> with <see langword="null"/> to the parameter.</returns>
    [PublicAPI]
    bool TryGetTranslation(string textContext, Plural plural, object[]? arguments, [MaybeNullWhen(false)] out string? translation);
}