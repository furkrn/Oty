namespace Oty.Interactivity.Entities;

public sealed class InteractionPage
{
    public InteractionPage(DiscordInteractionResponseBuilder responseBuilder)
    {
        this.ResponseBuilder = responseBuilder;
    }

    public DiscordInteractionResponseBuilder ResponseBuilder { get; }

    public static implicit operator DiscordInteractionResponseBuilder(InteractionPage page)
        => page!.ResponseBuilder;

    public static implicit operator DiscordWebhookBuilder(InteractionPage? page)
    {
        return new DiscordWebhookBuilder(page.ResponseBuilder);
    }
}