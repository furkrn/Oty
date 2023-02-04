namespace Oty.CommandLib.Interactions;

public sealed class SlashInteractionCommandOptionBuilder
{
    private readonly List<(Func<SlashInteractionCommandMetadataOptionBuilder, SlashInteractionCommandMetadataOptionBuilder> Action, ApplicationCommandOptionType Type)> _builders = new();

    internal SlashInteractionCommandOptionBuilder(int position)
    {
        this.Position = position;
    }

    public string? Name { get; private set; }

    public Optional<object?> DefaultValue { get; private set; }

    public Type? ParameterType { get; private set; }

    public int Position { get; }

    public SlashInteractionCommandOptionBuilder WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        this.Name = name;

        return this;
    }

    public SlashInteractionCommandOptionBuilder WithDefaultValue(object? value)
    {
        this.DefaultValue = Optional.FromValue(value);

        return this;
    }

    public SlashInteractionCommandOptionBuilder WithType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        this.ParameterType = Nullable.GetUnderlyingType(type) ?? type;

        return this;
    }

    public SlashInteractionCommandOptionBuilder WithType<T>()
    {
        return this.WithType(typeof(T));
    }

    public SlashInteractionCommandOptionBuilder WithMetadata(ApplicationCommandOptionType type, Func<SlashInteractionCommandMetadataOptionBuilder, SlashInteractionCommandMetadataOptionBuilder> actionBuilder)
    {
        ArgumentNullException.ThrowIfNull(actionBuilder, nameof(actionBuilder));
        this._builders.Add((actionBuilder, type));
        
        return this;
    }

    public SlashInteractionCommandOption Build()
    {
        if (this.ParameterType is null)
        {
            throw new InvalidOperationException("Parameter type must be specified!");
        }

        if (this._builders.Count == 0)
        {
            throw new InvalidOperationException("Least one metadata option must be provided with the option.");
        }  

        if (this.DefaultValue.IsDefined(out var value) && value is not null && !this.ParameterType.IsAssignableFrom(value.GetType()))
        {
            throw new InvalidOperationException("Specified default values does not match with the specified default value.");
        }

        if (this.Name is null)
        {
            throw new InvalidOperationException("Name cannot be null.");
        }

        var instance = new SlashInteractionCommandOption(this);

        instance.MetadataOptions = this._builders.Select((tuple, count) =>
        {
            var builder = new SlashInteractionCommandMetadataOptionBuilder(count, tuple.Type, !this.DefaultValue.HasValue);

            tuple.Action(builder);

            return builder.Build(instance);
        })
        .ToList();

        return instance;
    }

    public static implicit operator SlashInteractionCommandOption(SlashInteractionCommandOptionBuilder builder)
        => builder.Build();
}