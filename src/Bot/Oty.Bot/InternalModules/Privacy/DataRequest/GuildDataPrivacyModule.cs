namespace Oty.Bot.InternalModules;

public partial class PrivacyModule
{
    [UsedImplicitly]
    public sealed class GuildDataPrivacyModule : BaseDataRequestPrivacyModule
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IStringLocalizer<GuildDataPrivacyModule> _localizer;

        public GuildDataPrivacyModule(SlashInteractionContext context) : base(context)
        {
            this._scopeFactory = context.RegisteredServices!.GetRequiredService<IServiceScopeFactory>();
            this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<GuildDataPrivacyModule>>();
        }

        // resharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

        public override async Task<bool> BeforeExecutionAsync()
        {
            if (!await base.BeforeExecutionAsync())
            {
                return false;
            }

            if (this.Context.Guild is null)
            {
                var interaction = this.TosValidatedInteraction ?? this.Context.Interaction;

                DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["DataRequestNotInGuild", TranslationFindingType.WithOnlyTextContext])
                    .WithColor(DiscordColor.Red);

                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed));
                return false;
            }

            return true;
        }

        public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
        {
            var scopeFactory = this._scopeFactory;
            ulong guildId = this.Context.User.Id;

            return this.CreateDataResponseAsync(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                await using var repository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

                var (guild, _) = await repository.HasGuildAsync(guildId);
                return guild; 
            });
        }
    }
}