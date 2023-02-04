namespace Oty.Bot.Events;

public class CommandFailedEvent : IAsyncEventHandler<IOtyCommandsExtension, CommandHandledEventArgs>
{
    private readonly IStringLocalizer<CommandFailedEvent> _localizer;

    public CommandFailedEvent(IStringLocalizer<CommandFailedEvent> localizer)
    {
        this._localizer = localizer;
    }

    public async Task ExecuteAsync(IOtyCommandsExtension sender, CommandHandledEventArgs e)
    {
        if (e.ExecutionResult.IsExecuted || e.ExecutionResult.Exception is null ||
            e.ExecutionResult.Event is not InteractionCreateEventArgs ie)
        {
            return;
        }

        var result = await ie.Interaction.HasMessageAsync();

        string content = e.ExecutionResult.Exception switch
        {
            UsersFaultException ue => ue.InnerException!.Message,
            LimitedException le => le.Message,
            _ => $"{this._localizer["An internal error occured!"]} \n {e.ExecutionResult.Exception?.Message}.",
        };

        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithDescription(content)
            .WithColor(DiscordColor.Red);

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder()
            .AddEmbed(embed);

        if (result.IsSuccessfull)
        {  
            var followUpMessageBuilder = new DiscordFollowupMessageBuilder(interactionResponseBuilder.WithContent(this._localizer["Error was occurred after response:"]));

            await ie.Interaction.CreateFollowupMessageAsync(followUpMessageBuilder);
        }
        else
        {
            try
            {
                await ie.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, interactionResponseBuilder);
            }
            catch (BadRequestException)
            {
                await ie.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(interactionResponseBuilder));
            }
        }
    }
}