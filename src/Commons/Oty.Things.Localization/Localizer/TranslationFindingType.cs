namespace Oty.Things.Localization;

/// <summary>
/// Specifies what kind of why will be used for finding specific translation.
/// </summary>
[PublicAPI]
public enum TranslationFindingType
{
    /// <summary>
    /// Defines everything is required for finding the specific translation.
    /// </summary>
    WithSpecifyingEverything,

    /// <summary>
    /// Defines only <see cref="TextContext"/> is required for finding specific translation.
    /// </summary>
    WithOnlyTextContext,
}