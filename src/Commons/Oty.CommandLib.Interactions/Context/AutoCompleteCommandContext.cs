namespace Oty.CommandLib.Interactions;

/// <summary>
/// Represents context for autocomplete interactions.
/// </summary>
public sealed class AutoCompleteInteractionContext : BaseInteractionCommandContext
{
    internal AutoCompleteInteractionContext(DiscordClient client, IOtyCommandsExtension sender, AutoCompleteInteractionCommand metadata, DiscordInteraction interaction, IServiceProvider? serviceProvider) : base(client, sender, metadata, interaction, serviceProvider)
    {
    }

    public object? Value => this.Interaction.Data.Options.FirstOrDefault(o => o.Focused)?.Value;

    /// <summary>
    /// Sets the option that will be sent to Discord.
    /// </summary>
    [PublicAPI]
    public IEnumerable<DiscordAutoCompleteChoice>? Choices { internal get; set; }
}