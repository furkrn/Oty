namespace Oty.Bot.Addons.CommandLib;

public sealed class InteractionReceivedValue : IReceivedValue
{
    public InteractionReceivedValue(DiscordInteraction interaction)
    {
        this.Interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
    }

    public DiscordInteraction Interaction { get; }
}