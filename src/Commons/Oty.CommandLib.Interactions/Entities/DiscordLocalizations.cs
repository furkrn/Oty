namespace Oty.CommandLib.Interactions.Entities;

public sealed class DiscordLocalizations : IEquatable<DiscordLocalizations>
{
    public DiscordLocalizations(IReadOnlyDictionary<string, LocalizedValues> localizations)
    {
        this.Localizations = localizations ?? throw new ArgumentNullException(nameof(localizations));
    }

    public static DiscordLocalizations Empty = new(new Dictionary<string, LocalizedValues>());

    public IReadOnlyDictionary<string, LocalizedValues> Localizations { get; }

    public IReadOnlyDictionary<string, string> GetDescriptionLocalizations()
        => this.GetItemDictionary(l => l.Description);

    public IReadOnlyDictionary<string, string> GetNameLocalizations()
        => this.GetItemDictionary(l => l.Name);

    public override bool Equals(object? obj)
    {
        return obj is DiscordLocalizations other && this.Equals(other);
    }

    public bool Equals(DiscordLocalizations? other)
    {
        return other is not null &&
            EqualityComparer<IReadOnlyDictionary<string, LocalizedValues>>.Default.Equals(this.Localizations, other.Localizations);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Localizations);
    }

    public static bool operator ==(DiscordLocalizations left, DiscordLocalizations right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DiscordLocalizations left, DiscordLocalizations right)
    {
        return !(left == right);
    }

    private IReadOnlyDictionary<string, string> GetItemDictionary(Func<LocalizedValues, string?> selector)
    {
        return this.Localizations.Select(items => new { items.Key, Value = selector(items.Value) })
            .Where(k => k.Value is not null)
            .ToDictionary(k => k.Key, k => k.Value!);
    } 
}