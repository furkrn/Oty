namespace Oty.CommandLib;

/// <summary>
/// Represents context for commands.
/// </summary>
public abstract class BaseCommandContext
{
    /// <summary>
    /// Creates an instance of the base class with context received from handlers.
    /// </summary>
    protected BaseCommandContext(DiscordClient client, IOtyCommandsExtension sender, BaseCommandMetadata metadata, IServiceProvider? serviceProvider)
    {
        this.Client = client ?? throw new ArgumentNullException(nameof(client));
        this.Extension = sender ?? throw new ArgumentNullException(nameof(sender));
        this.Command = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.RegisteredServices = serviceProvider;
    }

    /// <summary>
    /// Gets the current Discord client.
    /// </summary>
    [PublicAPI]
    public DiscordClient Client { get; }

    /// <summary>
    /// Gets the registered services to the extension.
    /// </summary>
    [PublicAPI]
    public IServiceProvider? RegisteredServices { get; }

    /// <summary>
    /// Gets the current slash command extension.
    /// </summary>
    [PublicAPI]
    public IOtyCommandsExtension Extension { get; }

    /// <summary>
    /// Gets the application command that is being executed.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata Command { get; }
}