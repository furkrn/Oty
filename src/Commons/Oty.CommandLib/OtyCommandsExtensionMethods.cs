namespace Oty.CommandLib;

/// <summary>
/// Gets the extension methods to create or get instance of an <see cref="OtyCommandsExtension"/>
/// </summary>
[PublicAPI]
public static class OtyCommandsExtensionMethods
{
    /// <summary>
    /// Creates a new instance of <see cref="OtyCommandsExtension"/>
    /// </summary>
    /// <param name="client">Client to register this extension</param>
    /// <param name="configuration">Configuration for extension</param>
    [PublicAPI]
    public static IOtyCommandsExtension AddCommandsExtension(this DiscordClient client, OtyCommandsConfiguration? configuration = null)
    {
        var slashCommandExtension = new OtyCommandsExtension(configuration ?? new());

        client.AddExtension(slashCommandExtension);

        return slashCommandExtension;
    }

    /// <summary>
    /// Gets registered extension
    /// </summary>
    /// <param name="client"></param>
    /// <returns>Gets instance of extension if it's registered to <paramref name="client"/> else null</returns>
    [PublicAPI]
    public static IOtyCommandsExtension GetCommandsExtension(this DiscordClient client)
    {
        return client.GetExtension<OtyCommandsExtension>();
    }
}