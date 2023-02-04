namespace Oty.Bot;

[PublicAPI]
public static class DiscordClientExtensions
{
    [PublicAPI]
    public static Task ConnectAsync(this DiscordClient discordClient, DiscordStatus status)
        => discordClient.ConnectAsync(new(status.Activity, status.ActivityType), status.UserStatus);

    [PublicAPI]
    public static Task UpdateStatusAsync(this DiscordClient discordClient, DiscordStatus status)
        => discordClient.UpdateStatusAsync(new(status.Activity, status.ActivityType), status.UserStatus);
}