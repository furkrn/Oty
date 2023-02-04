namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Represents the information about the group.
/// </summary>
[PublicAPI]
public readonly struct Group<TCommand> : IDiscordGroup<TCommand>, IEquatable<Group<TCommand>>
    where TCommand : BaseCommandMetadata
{
    /// <summary>
    /// Creates an instance of a <see cref="Group{TCommand}"/>.
    /// </summary>
    /// <param name="command">Command on the group.</param>
    /// <param name="name">Name of the group.</param>
    /// <param name="description">Description of the group.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is <see langword="null"/>, empty or a whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="description"/> is <see langword="null"/>, empty or a whitespace.</exception>
    public Group(TCommand command, string name, string? description = "No Description Provided", DiscordLocalizations? localizations = null)
    {
        this.Subcommand = command ?? throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name of the group cannot be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description of the group cannot be null or whitespace.", nameof(description));
        }

        this.Name = name;
        this.Description = description;
        this.Localizations = localizations ?? DiscordLocalizations.Empty;
    }

    /// <inheritdoc/>
    public TCommand Subcommand { get; }

    /// <summary>
    /// Gets the description of the group.
    /// </summary>
    [PublicAPI]
    public string Description { get; }

    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    [PublicAPI]
    public string Name { get; }

    [PublicAPI]
    public DiscordLocalizations Localizations { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Group<TCommand> group && this.Equals(group);
    }

    /// <inheritdoc/>
    public bool Equals(Group<TCommand> other)
    {
        return this.Description == other.Description &&
            this.Name == other.Name &&
            this.Localizations == other.Localizations;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name, this.Description);
    }

    public static bool operator ==(Group<TCommand> left, Group<TCommand> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Group<TCommand> left, Group<TCommand> right)
    {
        return !(left == right);
    }
}