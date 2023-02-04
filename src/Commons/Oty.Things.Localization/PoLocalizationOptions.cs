namespace Oty.Things.Localization;

/// <summary>
/// Provides options for PO based localization.
/// </summary>
[PublicAPI]
public class PoLocalizationOptions : LocalizationOptions
{
    /// <summary>
    /// Gets/sets the settings for parsing PO files.
    /// </summary>
    /// <returns></returns>
    public POParserSettings ParserSettings { get; set; } = new()
    {
        SkipInfoHeaders = true,
        StringDecodingOptions = new POStringDecodingOptions() { KeepKeyStringsPlatformIndependent = true }
    };
}