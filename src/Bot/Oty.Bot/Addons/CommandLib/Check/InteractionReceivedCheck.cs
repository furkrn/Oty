namespace Oty.Bot.Addons.CommandLib;

public sealed class InteractionReceivedCheck : IReceivedCheck<InteractionReceivedValue>
{
    public IReceivedValue? NewReceived { get; private set; }

    public async Task<bool> CanExecute(InteractionReceivedValue value, AddonContext context)
    {
        var discordInteraction = value.Interaction;

        string targetButtonId = "agree";
        var agreeComponent = new DiscordButtonComponent(ButtonStyle.Success, targetButtonId, "Agree", false, new DiscordComponentEmoji("✅"));

        var embedBuilder = new DiscordEmbedBuilder()
            .WithDescription("To start using this bot, you have to agree our [Terms of Service](https://www.instagram.com/cnremek) by clicking the '✅Agree' button below. If you don't want too agree, you can close this message.")
            .WithColor(DiscordColor.Red);

        var firstTimeResponseBuilder = new DiscordInteractionResponseBuilder()
            .AddEmbed(embedBuilder)
            .AddComponents(agreeComponent)
            .AsEphemeral();

        await discordInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, firstTimeResponseBuilder);
        var originalMessage = await discordInteraction.GetOriginalResponseAsync();

        var interactionHandler = context.Client.GetInteractivityExtension();

        var clickRequest = new ClickInteractivityRequestBuilder()
            .SetTargetUser(discordInteraction.User)
            .WithTargetMessage(originalMessage)
            .WithTargetInteractionId(targetButtonId)
            .SetRepeative(false);

        var result = await interactionHandler.HandleClickRequestAsync(clickRequest, TimeSpan.FromMinutes(1));

        if (result.IsTimedOut || !result.IsExecuted)
        {
            return false;
        }

        this.NewReceived = new InteractionReceivedValue(result.EventResult.Interaction);

        return true;
    }
}