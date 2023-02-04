namespace Oty.Interactivity;

/// <summary>
/// Provides configuration for <see cref="OtyInteractivity"></see>
/// </summary>
[PublicAPI]
public sealed class OtyInteractivityConfiguration
{
    /// <summary>
    /// Gets or initalizes unauthorized action interaction message.
    /// </summary>
    [PublicAPI]
    public DiscordInteractionResponseBuilder NoPermissionResponseBuilder { get; init; } = new DiscordInteractionResponseBuilder()
        .AsEphemeral()
        .WithContent("🧨😕");
}