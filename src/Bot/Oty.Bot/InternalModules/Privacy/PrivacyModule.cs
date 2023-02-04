namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed partial class PrivacyModule : BaseCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    public PrivacyModule(SlashInteractionContext context) : base(context)
    {
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        if (metadataProvider is LimitedCommandMetadataProvider limitedMetadataProvider)
        {
            limitedMetadataProvider.AddLimit<UserDataPrivacyModule>(new LimitationInfo(LimitationTypes.UserWise, TimeSpan.FromMinutes(5), 1))
                .AddLimits<GuildDataPrivacyModule>(new LimitationInfo[]
                {
                    new(LimitationTypes.UserWise, TimeSpan.FromMinutes(5), 1),
                    new(LimitationTypes.GuildWise, TimeSpan.FromMinutes(10), 2),
                })
                .AddLimit<DataDeletionPrivacyModule.UserDataDeletionPrivacyModule>(new LimitationInfo(LimitationTypes.UserWise, TimeSpan.FromDays(1), 1))
                .AddLimit<DataDeletionPrivacyModule.GuildDataDeletionPrivacyModule>(new LimitationInfo(LimitationTypes.GuildWise, TimeSpan.FromDays(1), 1));
        }

        return new InteractionCommandBuilder<PrivacyModule>(ApplicationCommandType.SlashCommand)  // TODO : Localization is not added to group commands yet.
            .WithName("privacy")
            .AllowPrivateChannels()
            .WithDescription("Your privacy matters.")
            .WithPermissions(Permissions.UseApplicationCommands)
            .LocalizeFrom(metadataProvider)
            .AddSubcommand<UserDataPrivacyModule>(seeDataBuilder => seeDataBuilder.WithName("mydata")
                .WithDescription("Shows what is my data looks like")
                .LocalizeFrom(metadataProvider))
            .AddSubcommand<GuildDataPrivacyModule>(guildDataBuilder => guildDataBuilder.WithName("guilddata")
                .WithDescription("Sees what data collected from guild.")
                .LocalizeFrom(metadataProvider))
            .AddSubcommand<DataDeletionPrivacyModule>(dataDeletionBuilder => dataDeletionBuilder.WithName("delete")
                .WithDescription("Delete data related to me or my community")
                .LocalizeFrom(metadataProvider)
                .AddSubcommand<DataDeletionPrivacyModule.UserDataDeletionPrivacyModule>(userDeletionBuilder => userDeletionBuilder.WithName("user")
                    .WithDescription("Delete my data")
                    .LocalizeFrom(metadataProvider))
                .AddSubcommand<DataDeletionPrivacyModule.GuildDataDeletionPrivacyModule>(guildDeletionBuilder => guildDeletionBuilder.WithName("guild")
                    .WithDescription("Delete my community's data")
                    .LocalizeFrom(metadataProvider)));
    }
}