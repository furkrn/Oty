namespace Oty.CommandLib.Interactions;

/// <summary>
/// Builder class to build application command options.
/// </summary>
public sealed class SlashInteractionCommandMetadataOptionBuilder
{
    private readonly Dictionary<string, LocalizedValues> _localizations = new();

    private string _description = "No Description Provided";

    private string _name = string.Empty;

    private IEnumerable<ChannelType>? _channelTypes;

    private DiscordOptionRange? _range;

    private IChoiceProvider? _choiceProvider;

    private string? _autoCompleteCommand;

    internal SlashInteractionCommandMetadataOptionBuilder(int position, ApplicationCommandOptionType optionType, bool isRequired)
    {
        if (optionType is (ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup))
        {
            throw new ArgumentException("Option type of subcommand or subcommand group cannot be built using this builder.", nameof(optionType));
        }

        this.OptionType = optionType;
        this.Position = position;
        this.IsRequired = isRequired;
    }

    /// <summary>
    /// Gets the position of the argument.
    /// </summary>
    /// <value></value>
    public int Position { get; }

    /// <summary>
    /// Gets whether options is required or not.
    /// </summary>
    [PublicAPI]
    public bool IsRequired { get; }

    /// <summary>
    /// Gets or sets the name of the application option that will be created.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if specified value is <see langword="null"/> or a whitespace character.</exception>
    /// <exception cref="ArgumentException">Thrown if specified value is incorrect naming for option.</exception>
    [PublicAPI]
    public string Name
    {
        get
        {
            return this._name;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be whitespace or null");
            }

            if (!value.IsValidSlashCommandName())
            {
                throw new ArgumentException("Supplied option name is not valid.");
            }

            this._name = value;
        }
    }

    /// <summary>
    /// Gets or sets the description of the application option that will be created.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if specified value is <see langword="null"/> or a whitespace character</exception>
    /// <exception cref="ArgumentException">Thrown if specified value is longer than 100 characters.</exception>
    [PublicAPI]
    public string? Description
    {
        get
        {
            return this._description;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace.");
            }

            if (value.Length > 100)
            {
                throw new ArgumentException("Length of description can be maximum 100.");
            }

            this._description = value;
        }
    }

    /// <summary>
    /// Gets option's raw type that will be resolved, converted and created.
    /// </summary>
    [PublicAPI]
    public ApplicationCommandOptionType OptionType { get; }

    /// <summary>
    /// Gets or sets choice provider of the application option that will be created.
    /// <para>Only valid <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <exception cref="ArgumentException"><see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public IChoiceProvider? ChoiceProvider
    {
        get
        {
            return this._choiceProvider;
        }
        set
        {
            if (this.OptionType is not (ApplicationCommandOptionType.String or ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number))
            {
                throw new ArgumentException("Choice providers are not supported for this type.");
            }

            this._choiceProvider = value;
        }
    }

    /// <summary>
    /// Gets or sets the autocomplete provider of the application option that will be created.
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <exception cref="ArgumentException"><see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public string? AutoCompleteCommand
    {
        get
        {
            return this._autoCompleteCommand;
        }
        set
        {
            if (this.OptionType is not (ApplicationCommandOptionType.String or ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number))
            {
                throw new ArgumentException("Autocomplete providers are not supported for this type.");
            }

            this._autoCompleteCommand = value;
        }
    }

    /// <summary>
    /// Gets or sets the channel type of the application option that will be created.
    /// <para>You can set this property when the <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Channel"/></para>
    /// </summary>
    /// <exception cref="ArgumentException">If <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Channel"/></exception>
    [PublicAPI]
    public IEnumerable<ChannelType>? ChannelTypes
    {
        get
        {
            return this._channelTypes;
        }
        set
        {
            if (this.OptionType != ApplicationCommandOptionType.Channel)
            {
                throw new ArgumentException("Channel Types can be only used with option type of Channel.");
            }

            this._channelTypes = value;
        }
    }

    /// <summary>
    /// Gets or the sets the minimum and maximum that option can go.
    /// <para>Ranges can be set for only <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public DiscordOptionRange? Range
    {
        get
        {
            return this._range;
        }
        set
        {
            if (this.OptionType is not (ApplicationCommandOptionType.Number or ApplicationCommandOptionType.Integer))
            {
                throw new ArgumentException("Range can be set for only integer and number raw types.");
            }

            this._range = value;
        }
    }

    [PublicAPI]
    public DiscordLocalizations Localizations => new(this._localizations);

    public SlashInteractionCommandMetadataOptionBuilder AddLocalizedValues(string culture, string? name, string? description)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        return this.AddLocalizedValues(culture, LocalizedValues.Create(name, description));
    }

    public SlashInteractionCommandMetadataOptionBuilder AddLocalizedValues(string culture, LocalizedValues values)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        this._localizations.Add(culture, values);

        return this;
    }

    /// <summary>
    /// Sets the autocomplete command of the option.
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <param name="command">The autocomplete that is created.</param>
    /// <exception cref="ArgumentException"><see cref="OptionType"/> isn't a <see cref="ApplicationCommandOptionType.String"/> or <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithAutoCompleteCommand(string commandName)
    {
        this.AutoCompleteCommand = commandName;

        return this;
    }

    /// <summary>
    /// Sets the description of the option.
    /// </summary>
    /// <param name="description">Description to set.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="description"/> is <see langword="null"/> or a whitespace character</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="description"/> is longer than 100 characters.</exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithDescription(string description)
    {
        this.Description = description;

        return this;
    }

    /// <summary>
    /// Sets the name of the option.
    /// </summary>
    /// <param name="name">Name to set.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is <see langword="null"/> or a whitespace character.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is incorrect naming for option.</exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithName(string name)
    {
        this.Name = name;

        return this;
    }

    /// <summary>
    /// Sets the minimum and maximum values opiton can go values for the option.
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <param name="range">Minimum and maximum values to set.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="range"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown If <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithRange(DiscordOptionRange? range)
    {
        this.Range = range;

        return this;
    }

    /// <summary>
    /// Sets the minimum and maximum values that option can go.
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <param name="minimum">Minimum value to set.</param>
    /// <param name="maximum">Maximum value to set.</param>
    /// <exception cref="ArgumentException">Thrown If <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithRange(long? minimum, long? maximum)
    {
        return this.WithRange(new(minimum, maximum));
    }

    /// <summary>
    /// Sets the minimum and maximum values that option can go.
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    /// <exception cref="ArgumentException">Thrown If <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithRange(double? minimum, double? maximum)
    {
        return this.WithRange(new(minimum, maximum));
    }

    /// <summary>
    /// Sets the channels that option can display
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Channel"/></para>
    /// </summary>
    /// <param name="channelTypes">Sets the channels that option can display.</param>
    /// <exception cref="ArgumentException">If <see cref="OptionType"/> isn't <see cref="ApplicationCommandOptionType.Channel"/></exception>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithChannelTypes(params ChannelType[]? channelTypes)
    {
        this.ChannelTypes = channelTypes;

        return this;
    }

    /// <summary>
    /// Sets the channels that option can display
    /// <para>Only valid if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Channel"/></para>
    /// </summary>
    /// <param name="channelTypes">Sets the channels that option can display.</param>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithChannelTypes(IEnumerable<ChannelType>? channelTypes)
    {
        return this.WithChannelTypes(channelTypes?.ToArray());
    }

    /// <summary>
    /// Sets the choice provider that option can go.
    /// </summary>
    /// <param name="choiceProvider">Choice provider to set.</param>
    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithChoiceProvider(IChoiceProvider choiceProvider)
    {
        this.ChoiceProvider = choiceProvider;

        return this;
    }

    [PublicAPI]
    public SlashInteractionCommandMetadataOptionBuilder WithLocalizationProvider(ILocalizationProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        foreach (var (culture, values) in provider.GetLocalizations())
        {
            this.AddLocalizedValues(culture, values);
        }

        return this;
    }

    /// <summary>
    /// Builds an instance of <see cref="SlashInteractionCommandOptionMetadata"/> based on values on this builder.
    /// </summary>
    /// <returns>An instance of <see cref="SlashInteractionCommandOptionMetadata"/></returns>
    /// <exception cref="InvalidOperationException">Thrown when type <see cref="Range"/> set incorrectly.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="option"/> is <see langword="null"/></exception>
    [PublicAPI]
    public SlashInteractionCommandOptionMetadata Build(IDiscordOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (string.IsNullOrWhiteSpace(this._name))
        {
            throw new InvalidOperationException("Options must contain its name.");
        }

        return new(this, option);
    }
}