namespace Oty.Bot.Addons.CommandLib;

public sealed class AddonContext : BaseCommandContext
{
    internal AddonContext(DiscordClient client, IOtyCommandsExtension extension, AddonMetadata metadata, IServiceProvider? serviceProvider, IReceivedValue received) : base(client, extension, metadata, serviceProvider)
    {
        this.Received = received ?? throw new ArgumentNullException(nameof(received));
    }

    public IReceivedValue Received { get; }
}