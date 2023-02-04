namespace Oty.Bot.InternalModules;

public partial class PrivacyModule
{
    public partial class DataDeletionPrivacyModule
    {
        [UsedImplicitly]
        public sealed class GuildDataDeletionPrivacyModule : BaseDataDeletionPrivacyModule
        {
            private readonly IStringLocalizer<GuildDataDeletionPrivacyModule> _localizer;

            private readonly IOptionsSnapshot<BotConfiguration> _configuration;

            private DiscordInteraction? _interaction;

            public GuildDataDeletionPrivacyModule(SlashInteractionContext context) : base(context)
            {
                this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<GuildDataDeletionPrivacyModule>>();
                this._configuration = context.RegisteredServices!.GetRequiredService<IOptionsSnapshot<BotConfiguration>>();
            }

            // resharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

            public override async Task<bool> BeforeExecutionAsync()
            {
                CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

                if (this.Context.Guild is null)
                {
                    await this.CreateNotInGuildMessage();

                    return false;
                }

                if (!await base.BeforeExecutionAsync() || this.Context.Guild.Id == this._configuration.Value.SpecialGuildId)
                {
                    return false;
                }

                if (!CanRemove())
                {
                    await this.CreateNoPermissionsMessage();

                    return false;
                }

                DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle(this._localizer["GuildRemovalQuestionTitle", TranslationFindingType.WithOnlyTextContext])
                    .WithDescription(this._localizer["GuildRemovalQuestionDescription", TranslationFindingType.WithOnlyTextContext]);

                string leaveButtonId = "leave";
                var leaveButton = new DiscordButtonComponent(ButtonStyle.Danger, leaveButtonId, this._localizer["LeaveButtonText", TranslationFindingType.WithSpecifyingEverything], false, new("👋"));

                var interactionResponseBuilder = new DiscordInteractionResponseBuilder()
                    .AddComponents(leaveButton)
                    .AddEmbed(embed)
                    .AsEphemeral();

                await this.ValidatedInteraction!.CreateResponseAsync(InteractionResponseType.UpdateMessage, interactionResponseBuilder);

                var message = await this.ValidatedInteraction!.GetOriginalResponseAsync();

                var interactivityBuilder = new ClickInteractivityRequestBuilder()
                    .WithTargetMessage(message)
                    .SetTargetUser(this.Context.User)
                    .WithTargetInteractionId(leaveButtonId);

                var result = await this.Context.Client.GetInteractivityExtension()
                    .HandleClickRequestAsync(interactivityBuilder, TimeSpan.FromMinutes(1));

                this._interaction = result.EventResult.Interaction;

                return result.IsExecuted && !result.IsTimedOut;

                bool CanRemove()
                {
                    var member = this.Context.Member;
                    return member!.IsOwner ||
                        member.Permissions.HasPermission(Permissions.ManageGuild);
                }
            }

            public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
            {
                CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

                await this._interaction!.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder()
                    .AsEphemeral());

                await using var scope = this.ScopeFactory.CreateAsyncScope();
                await using var repository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

                var guild = await repository.HasGuildAsync(this.Context.GuildId!.Value);

                if (!guild.IsSuccessfull)
                {
                    await this.CreateNotAvaliableMessage();
                    return;
                }

                await repository.RemoveGuildAsync(guild!);
                await repository.UnitOfWork.SaveChangesAsync();

                var embed = new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["GuildRemovedDescription", TranslationFindingType.WithOnlyTextContext])
                    .WithColor(DiscordColor.Blurple);

                await this._interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(embed));

                await this.Context.Guild.LeaveAsync();
            }

            private Task CreateNotInGuildMessage()
            {
                DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["GuildDeletionNotInGuild", TranslationFindingType.WithOnlyTextContext]);

                return this.Context.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AsEphemeral());
            }

            private Task CreateNoPermissionsMessage()
            {
                DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["GuildDeletionNoPermission", TranslationFindingType.WithOnlyTextContext]);

                return this.ValidatedInteraction!.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AsEphemeral());
            }

            private Task CreateNotAvaliableMessage()
            {
                DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["GuildDeletionNotInGuild", TranslationFindingType.WithOnlyTextContext])
                    .WithColor(DiscordColor.Blurple);

                return this.ValidatedInteraction!.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AsEphemeral());
            }
        }
    }
}