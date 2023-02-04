namespace Oty.Bot.Addons.CommandLib;

public sealed class CommandMetadataProvider : IMetadataProvider
{
    public CommandMetadataProvider(IServiceProvider? services, ulong guildId)
    {
        this.Services = services;
        this.GuildId = guildId;
    }

    public IServiceProvider? Services { get; }

    public ulong GuildId { get; }
}