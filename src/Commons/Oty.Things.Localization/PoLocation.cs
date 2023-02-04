namespace Oty.Things.Localization;

/// <summary>
/// Provides information about the po file.
/// </summary>
[PublicAPI]
public readonly struct PoLocation
{
    /// <summary>
    /// Creates an instance of <see cref="PoLocation"/>
    /// </summary>
    /// <param name="location">The file location of the PO file.</param>
    /// <param name="locale">The culture that PO file depends.</param>
    /// <param name="isSuccessfull">Whether parsed PO file occurred and error or not.</param>
    [PublicAPI]
    public PoLocation(string location, string locale, bool isSuccessfull)
    {
        this.Location = location;
        this.Locale = locale;
        this.IsSuccessfull = isSuccessfull;
    }

    /// <summary>
    /// Gets the file location of the PO file.
    /// </summary>
    [PublicAPI]
    public string Location { get; }

    /// <summary>
    /// Gets the culture name of the PO file.
    /// </summary>
    [PublicAPI]
    public string Locale { get; }

    /// <summary>
    /// Gets whether PO file has parsed successfully or not.
    /// </summary>
    [PublicAPI]
    public bool IsSuccessfull { get; }
}