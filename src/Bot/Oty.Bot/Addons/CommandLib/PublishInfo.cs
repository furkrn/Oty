namespace Oty.Bot.Addons.CommandLib;

public struct PublishInfo
{
    public PublishInfo(bool isPublished, ulong id, AddonType type, DiscordApplicationCommand? command = null)
    {
        this.IsPublished = isPublished;
        this.Id = id;
        this.Type = type;
        this.Command = command;
    }

    public bool IsPublished { get; }

    public ulong Id { get; }

    public AddonType Type { get; }

    public DiscordApplicationCommand? Command { get; }
}