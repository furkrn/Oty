namespace Oty.CommandLib.Interactions;

[PublicAPI]
public abstract class InteractionCommandBuilder
{
    private protected InteractionCommandBuilder()
    {
    }

    /// <summary>
    /// Gets and sets the command name on this builder.
    /// </summary>
    /// <exception cref="ArgumentException">If the specified value is wrong slash command name.</exception>
    public abstract string CommandName { get; set; }

    /// <summary>
    /// Gets and sets the description of the command. Setting this value is only valiable to <see cref="ApplicationCommandType.SlashCommand"/>
    /// </summary>
    /// <exception cref="ArgumentException">If the specified <see cref="ApplicationCommandOptionType"/> on this builder isn't <see cref="ApplicationCommandType.SlashCommand"/></exception>
    /// <exception cref="ArgumentException">If value tried to set is <see langword="null"/> or an whitespace character.</exception>,
    /// <exception cref="ArgumentException">If the length of the value is longer than 100 characters.</exception>
    public abstract string? Description { get; set; }

     /// <summary>
    /// Gets or Sets the target guild to register command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <see cref="CommandType"/> is <see cref="ApplicationCommandType.AutoCompleteRequest"/></exception>
    public abstract ulong? GuildId { get; set; }

    /// <summary>
    /// Gets the type of the application that will be created.
    /// </summary>
    public ApplicationCommandType CommandType { get; protected init; }

    /// <summary>
    /// Gets or sets required permissions for using the application command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <see cref="CommandType"/></exception>
    public abstract Permissions CommandPermissions { get; set; }

    /// <summary>
    /// Gets or Sets whether command is enabled by default when if it's added to a guild.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <see cref="CommandType"/> is <see cref="ApplicationCommandType.AutoCompleteRequest"/></exception>
    public abstract bool DefaultPermission { get; set; }

    /// <summary>
    /// Indicates is command can be executed from application's private channels.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <see cref="CommandType"/> is <see cref="ApplicationCommandType.AutoCompleteRequest"/></exception>
    public abstract bool IsPrivateChannelsAllowed { get; set; }

    [PublicAPI]
    public abstract DiscordLocalizations Localizations { get; }

    /// <summary>
    /// Gets the options that is added to this builder.
    /// </summary>
    [PublicAPI]
    public abstract IReadOnlyList<SlashInteractionCommandOption> Options { get; }

    /// <summary>
    /// Gets the subcommands that is added to this builder.
    /// </summary>
    [PublicAPI]
    public abstract IReadOnlyList<IGroup<SlashInteractionCommand>> Subcommands { get; }
}

/// <summary>
/// A builder class to build <seealso cref="BaseCommandMetadata"/> depends on which <see cref="ApplicationCommandType"/> is used.
/// </summary>
[PublicAPI]
public sealed class InteractionCommandBuilder<TModule> : InteractionCommandBuilder
    where TModule : BaseCommandModule
{
    private string _commandName = string.Empty;

    private string? _description;

    private ulong? _guildId;

    private Permissions _permissions;

    private bool _allowPrivateChannels = true;

    private int _metadataCount;

    private readonly List<SlashInteractionCommandOption> _options = new();

    private readonly List<IGroup<SlashInteractionCommand>> _subCommands = new(25);

    private readonly Dictionary<string, LocalizedValues> _localizations = new();

    private bool _defaultPermission = true;

    /// <summary>
    /// Creates instance of it with specifying which type of command will be created.
    /// </summary>
    /// <param name="commandType">Type of application command that will be created.</param>
    public InteractionCommandBuilder(ApplicationCommandType commandType)
    {
        if (commandType is ApplicationCommandType.AutoCompleteRequest && !typeof(BaseAutoCompleteModule).IsAssignableFrom(typeof(TModule)))
        {
            throw new InvalidOperationException($"Autocomplete commands must implement {nameof(BaseAutoCompleteModule)}");
        }

        this.CommandType = commandType;
    }

    /// </inheritdoc>
    [PublicAPI]
    public override string? Description
    {
        get
        {
            return this._description;
        }
        set
        {
            if (this.CommandType != ApplicationCommandType.SlashCommand)
            {
                throw new ArgumentException("Descriptions are only supported on slash commands.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            if (value.Length > 100)
            {
                throw new ArgumentException("Description must be maximum 100 chars");
            }

            this._description = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override string CommandName
    {
        get
        {
            return this._commandName;
        }
        set
        {
            if (this.CommandType == ApplicationCommandType.SlashCommand && !value.IsValidSlashCommandName())
            {
                throw new ArgumentException("Invalid slash command name, slash commands must not contain qhitespace or upper chars and can be maximum 32 chars");
            }

            this._commandName = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override ulong? GuildId
    {
        get
        {
            return this._guildId;
        }
        set
        {
            if (this.CommandType == ApplicationCommandType.AutoCompleteRequest)
            {
                throw new ArgumentException("Autocomplete commands cannot have specified guild id.");
            }

            this._guildId = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override Permissions CommandPermissions
    {
        get
        {
            return this._permissions;
        }
        set
        {
            if (this.CommandType == ApplicationCommandType.AutoCompleteRequest)
            {
                throw new ArgumentException("Autocomplete commands cannot have specified permissions.");
            }

            this._permissions = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override bool DefaultPermission
    {
        get
        {
            return this._defaultPermission;
        }
        set
        {
            if (this.CommandType == ApplicationCommandType.AutoCompleteRequest)
            {
                throw new ArgumentException("Autocomplete commands cannot have specified default permission.");
            }

            this._defaultPermission = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override bool IsPrivateChannelsAllowed
    {
        get
        {
            return this._allowPrivateChannels;
        }
        set
        {
            if (this.CommandType == ApplicationCommandType.AutoCompleteRequest)
            {
                throw new ArgumentException("Autocomplete commands cannot have specified private channel allowment.");
            }

            this._allowPrivateChannels = value;
        }
    }

    /// </inheritdoc>
    [PublicAPI]
    public override IReadOnlyList<SlashInteractionCommandOption> Options => this._options;

    /// </inheritdoc>
    [PublicAPI]
    public override IReadOnlyList<IGroup<SlashInteractionCommand>> Subcommands => this._subCommands;

    /// </inheritdoc>
    [PublicAPI]
    public override DiscordLocalizations Localizations => new(this._localizations);

    public InteractionCommandBuilder<TModule> AddLocalizedValues(string culture, string? name, string? description)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        return this.AddLocalizedValues(culture, LocalizedValues.Create(name, description));
    }

    public InteractionCommandBuilder<TModule> AddLocalizedValues(string culture, LocalizedValues values)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        this._localizations.Add(culture, values);

        return this;
    }

    /// <summary>
    /// Adds a subcommand to this builder if it's valiable.
    /// </summary>
    /// <param name="subcommand">Subcommand to add.</param>
    /// <exception cref="ArgumentException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddSubcommand(SlashInteractionCommand subcommand)
    {
        ArgumentNullException.ThrowIfNull(subcommand);

        if (this.CommandType != ApplicationCommandType.SlashCommand)
        {
            throw new ArgumentException("Subcommands is only usable on slash commands");
        }

        if (this._options.Count > 0)
        {
            throw new ArgumentException("Subcommands and options cannot be used together");
        }

        if (this._subCommands.Count == 25)
        {
            throw new ArgumentException("Cannot contain more than 25 subcommands for a command.");
        }

        var groupInfo = new Group<SlashInteractionCommand>(subcommand, subcommand.Name, subcommand.Description, subcommand.Localizations);

        this._subCommands.Add(groupInfo);

        return this;
    }

    /// <summary>
    /// Adds subcommand to this builder using another builder.
    /// </summary>
    /// <param name="subcommandBuilderAction">Builder to build subcommand.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddSubcommand<TSubModuleType>(Action<InteractionCommandBuilder<TSubModuleType>> subcommandBuilderAction)
        where TSubModuleType : BaseCommandModule
    {
        ArgumentNullException.ThrowIfNull(subcommandBuilderAction, nameof(subcommandBuilderAction));

        if (this.CommandType != ApplicationCommandType.SlashCommand)
        {
            throw new ArgumentException("Subcommands is only usable on slash commands");
        }

        if (this._options.Count > 0)
        {
            throw new ArgumentException("Subcommands and options cannot be used together");
        }

        var subcommandBuilder = new InteractionCommandBuilder<TSubModuleType>(ApplicationCommandType.SlashCommand);
        
        subcommandBuilderAction(subcommandBuilder);

        return this.AddSubcommand((SlashInteractionCommand)subcommandBuilder.Build());
    }

    /// <summary>
    /// Adds subcommands specified in the collection to this builder.
    /// </summary>
    /// <param name="commandsArray">An array of the subcommands.</param>
    /// <exception cref="ArgumentException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddSubcommands(params SlashInteractionCommand[] commandsArray)
    {
        if (this._options.Count > 0)
        {
            throw new ArgumentException("Options and subcommands cannot be used together");
        }

        if (commandsArray.Any(c => c is null))
        {
            throw new ArgumentException("Any value in the collection cannot be null", nameof(commandsArray));
        }

        foreach (var command in commandsArray)
        {
            this.AddSubcommand(command);
        }

        return this;
    }

    /// <summary>
    /// Adds subcommands specified in the collection to this builder.
    /// </summary>
    /// <param name="commands">An Enumerable of the subcommands</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddSubcommands(IEnumerable<SlashInteractionCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        return this.AddSubcommands(commands.ToArray());
    }

    /// <summary>
    /// Adds options to this builder if it's valiable.
    /// </summary>
    /// <param name="option">Argument to add to the slaslh application command</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddOption(SlashInteractionCommandOption option)
    {
        if (this.CommandType != ApplicationCommandType.SlashCommand)
        {
            throw new ArgumentException("Options are only avaliable to slash commands.");
        }

        if (this._subCommands.Count > 0)
        {
            throw new ArgumentException("Subcommands and groups cannot be used together.");
        }

        if (this._metadataCount == 25)
        {
            throw new ArgumentException("Cannot store more than 25 options");
        }

        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (this._options.Any(opt => opt.Name == option.Name))
        {
            throw new ArgumentException("Supplied option has the same name of another option.", nameof(option));
        }

        var lastOption = this._options.LastOrDefault();

        if (InteractionCommandBuilderUtilityExtensions.IsOptionRequirementOrderingNotCorrect(option, lastOption))
        {
            throw new ArgumentException("Required options must be added before non-required options.");
        }

        this._metadataCount += option.MetadataOptions.Count;

        this._options.Add(option);

        return this;
    }

    /// <summary>
    /// Adds of option to this builder using an option builder if it's valiable.
    /// </summary>
    /// <param name="optionType">Type of the option to build.</param>
    /// <param name="optionBuilderAction">Option builder to build the command.</param>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AddOption(Action<SlashInteractionCommandOptionBuilder> optionBuilderAction)
    {
        if (this.CommandType != ApplicationCommandType.SlashCommand)
        {
            throw new ArgumentException("Options are only avaliable to slash commands.");
        }

        ArgumentNullException.ThrowIfNull(optionBuilderAction, nameof(optionBuilderAction));

        if (this._subCommands.Count > 0)
        {
            throw new ArgumentException("Grouped slash commands cannot have options.");
        }
        
        var optionBuilder = new SlashInteractionCommandOptionBuilder(this._options.Count);

        optionBuilderAction(optionBuilder);

        return this.AddOption(optionBuilder.Build());
    }

    /// <summary>
    /// Sets whether command can be executed from application's private channels.
    /// </summary>
    /// <param name="allow"></param>
    /// <exception cref="ArgumentException"/>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> AllowPrivateChannels(bool allow = true)
    {
        this.IsPrivateChannelsAllowed = allow;

        return this;
    }

    /// <summary>
    /// Sets the name of the application that will be created.
    /// </summary>
    /// <param name="name">A valid application command name.</param>
    /// <exception cref="ArgumentException">If the specified value is wrong slash command name.</exception>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithName(string name)
    {
        this.CommandName = name;

        return this;
    }

    /// <summary>
    /// Sets the description of this builder if it's valiable.
    /// </summary>
    /// <param name="description">The Slash application command description with maximum 100 chars.</param>
    /// <exception cref="ArgumentException">If the specified <see cref="ApplicationCommandOptionType"/> on this builder isn't <see cref="ApplicationCommandType.SlashCommand"/></exception>
    /// <exception cref="ArgumentException">If value tried to set is <see langword="null"/> or an whitespace character.</exception>,
    /// <exception cref="ArgumentException">If the length of the value is longer than 100 characters.</exception>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithDescription(string description)
    {
        this.Description = description;

        return this;
    }

    /// <summary>
    /// Sets whether command is enabled by default when if it's added to a guild.
    /// </summary>
    /// <param name="defaultPermission">Boolean to Enable command by default when it's added to guild</param>
    /// <exception cref="ArgumentException">Thrown if <see cref="CommandType"/> is <see cref="ApplicationCommandType.AutoCompleteRequest"/></exception>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithDefaultPermission(bool defaultPermission)
    {
        this.DefaultPermission = defaultPermission;

        return this;
    }

    /// <summary>
    /// Sets the target guild's id of the command.
    /// </summary>
    /// <param name="guildId">Guild id to set.</param>
    /// <exception cref="ArgumentException"></exception>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithGuildId(ulong? guildId)
    {
        this.GuildId = guildId;

        return this;
    }

    /// <summary>
    /// Sets the required permissions for using the application command.
    /// </summary>
    /// <param name="permissions">Permissions of the command to set.</param>
    /// <exception cref="ArgumentException"></exception>
    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithPermissions(Permissions permissions)
    {
        this.CommandPermissions = permissions;

        return this;
    }

    [PublicAPI]
    public InteractionCommandBuilder<TModule> WithLocalizationProvider(ILocalizationProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        foreach (var (culture, values) in provider.GetLocalizations())
        {
            this.AddLocalizedValues(culture, values);
        }

        return this;
    }

    /// <summary>
    /// Creates instance of an <see cref="BaseCommandMetadata"/> using values on this builder.
    /// </summary>
    /// <exception cref="ArgumentException">If it doesn't contain subgroups and no method is set.</exception>
    /// <exception cref="InvalidOperationException">If the specified <see cref="ApplicationCommandType"/> is not capable of building a command.</exception>
    [PublicAPI]
    public BaseCommandMetadata Build()
    {
        return this.CommandType switch
        {
            ApplicationCommandType.SlashCommand => new SlashInteractionCommand(this, typeof(TModule)),
            ApplicationCommandType.UserContextMenu or ApplicationCommandType.MessageContextMenu => new ContextMenuInteractionCommand(this, typeof(TModule)),
            ApplicationCommandType.AutoCompleteRequest => new AutoCompleteInteractionCommand(this, typeof(TModule)),
            _ => throw new InvalidOperationException("This type of application command option type isn't supported or not implemented yet."),
        };
    }

    public static implicit operator BaseCommandMetadata(InteractionCommandBuilder<TModule> builder)
        => builder.Build();

    /// <summary>
    /// Clears all of the values specified on this builder.
    /// </summary>
    [PublicAPI]
    public void Clear()
    {
        this._commandName = string.Empty;
        this._description = "No Description Provided";
        this._guildId = null;
        this._permissions = default;
        this._allowPrivateChannels = true;
        this._options.Clear();
        this._subCommands.Clear();
        this._defaultPermission = true;
    }
}