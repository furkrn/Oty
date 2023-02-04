namespace Oty.Bot.Utilities;

public static class DiscordInteractionExtensions
{
    public static async Task<Result<DiscordMessage>> HasMessageAsync(this DiscordInteraction interaction)
    {
        try
        {
            var message = await interaction.GetOriginalResponseAsync()
                .WaitAsync(TimeSpan.FromMilliseconds(300));

            return new Result<DiscordMessage>(true, message);
        }
        catch (TimeoutException)
        {
            return new Result<DiscordMessage>(false, null);
        }
    } 
}