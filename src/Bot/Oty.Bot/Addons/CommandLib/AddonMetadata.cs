namespace Oty.Bot.Addons.CommandLib;

public class AddonMetadata : BaseCommandMetadata
{
    public AddonMetadata(string name, ulong id, AddonType type) : base(name, typeof(AddonModule))
    {
        this.AddonId = id;
        this.Type = type;
    }

    public ulong AddonId { get; }

    public AddonType Type { get; }

    public PublishInfo? PublishedInfo { get; set; }
}