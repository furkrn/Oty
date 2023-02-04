namespace Oty.Things.Localization;

/// <summary>
/// Class to provide both location and catalog of the specified *.PO file.
/// </summary>
[PublicAPI]
public record struct PoEntry(POCatalog? Catalog, PoLocation Location);