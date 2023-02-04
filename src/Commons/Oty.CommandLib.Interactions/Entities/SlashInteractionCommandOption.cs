namespace Oty.CommandLib.Interactions.Entities;

public sealed class SlashInteractionCommandOption : IDiscordOption, IEquatable<SlashInteractionCommandOption>
{
    #nullable disable
    
    internal SlashInteractionCommandOption(SlashInteractionCommandOptionBuilder builder)
    {
        this.DefaultValue = builder.DefaultValue;
        this.Name = builder.Name!;
        this.Position = builder.Position;
        this.Type = builder.ParameterType!;
    }

    #nullable enable

    public Optional<object?> DefaultValue { get; }

    public string Name { get; }

    public int Position { get; }

    public IReadOnlyList<IMetadataOption> MetadataOptions { get; internal set; }

    public Type Type { get; }

    public IEnumerable<DiscordApplicationCommandOption> CreateOptions()
    {
        return this.MetadataOptions.Cast<IDiscordMetadataOption>()
            .Select(c => c.ToDiscordOption());
    }

    public bool Equals(SlashInteractionCommandOption? other)
    {
        return other is not null && 
            this.DefaultValue == other.DefaultValue &&
            this.Name == other.Name &&
            this.Position == other.Position &&
            EqualityComparer<IReadOnlyList<IMetadataOption>>.Default.Equals(this.MetadataOptions, other.MetadataOptions) &&
            this.Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        return obj is SlashInteractionCommandOption option && this.Equals(option);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.DefaultValue, this.Name, this.Position, this.MetadataOptions, this.Type);
    }

    public static bool operator ==(SlashInteractionCommandOption left, SlashInteractionCommandOption right)
    {
        return right.Equals(left);
    }

    public static bool operator !=(SlashInteractionCommandOption left, SlashInteractionCommandOption right)
    {
        return !(left == right);
    }
}