namespace Oty.Bot.InternalModules;

[PublicAPI]
public abstract class BaseDataRequestPrivacyModule : BaseVerifiedCommandModule<SlashInteractionContext>
{
    private readonly IStringLocalizer<BaseDataRequestPrivacyModule> _localizer;

    protected BaseDataRequestPrivacyModule(SlashInteractionContext context) : base(context)
    {
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<BaseDataRequestPrivacyModule>>();
    }

    protected async Task CreateDataResponseAsync<TValue>(Func<Task<TValue?>> stringFunc)
    {
        ArgumentNullException.ThrowIfNull(stringFunc, nameof(stringFunc));

        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        await this.Context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .AsEphemeral());

        var value = await stringFunc();

        if (value is null)
        {
            return;
        }

        string json = await JsonSerialization.SerializeAsync(value);
        string title = this._localizer["Your data is here:"];

        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithTitle(title)
            .WithDescription($"```\n {json}```");

        await this.Context.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(embed));
    }
}