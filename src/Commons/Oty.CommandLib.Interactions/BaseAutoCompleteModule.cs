namespace Oty.CommandLib.Interactions;

public abstract class BaseAutoCompleteModule : BaseCommandModule<AutoCompleteInteractionContext>
{
    protected BaseAutoCompleteModule(AutoCompleteInteractionContext context) : base(context)
    {
    }

    public sealed override async Task AfterExecutionAsync()
    {
        if (this.Context!.Choices is null)
        {
            throw new InvalidOperationException("Choices cannot be null, least provide an empty collection of it.");
        }

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder()    
            .AddAutoCompleteChoices(this.Context.Choices);

        await this.Context.Interaction.CreateResponseAsync(InteractionResponseType.AutoCompleteResult, interactionResponseBuilder).ConfigureAwait(false);
    }
}