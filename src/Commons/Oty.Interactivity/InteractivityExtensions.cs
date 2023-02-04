namespace Oty.Interactivity;

public static class InteractivityExtensions
{
    [PublicAPI]
    public static OtyInteractivity AddInteractivity(this DiscordClient client, OtyInteractivityConfiguration? configuration = null)
    {
        var interactivity = new OtyInteractivity(configuration ?? new());

        client.AddExtension(interactivity);

        return interactivity;
    }

    [PublicAPI]
    public static OtyInteractivity GetInteractivityExtension(this DiscordClient client)
    {
        return client.GetExtension<OtyInteractivity>();
    }
}