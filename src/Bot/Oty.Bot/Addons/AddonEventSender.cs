namespace Oty.Bot.Addons;

public sealed class AddonEventSender : IAddonEventSender
{
    private readonly DiscordClient _client;

    public AddonEventSender(DiscordClient client)
    {
        this._client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public void Dispose()
    {
    }
}