namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed partial class AddonManager : BaseVerifiedCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    public AddonManager(SlashInteractionContext context) : base(context)
    {
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        if (metadataProvider is LimitedCommandMetadataProvider limitedMetadataProvider)
        {
            var additionLimits = new[]
            {
                new LimitationInfo(LimitationTypes.UserWise, TimeSpan.FromSeconds(3), 1),
                new LimitationInfo(LimitationTypes.GuildWise, TimeSpan.FromSeconds(30), 1)
            };

            var standartLimit = new LimitationInfo(LimitationTypes.UserWise, TimeSpan.FromSeconds(3), 1);

            limitedMetadataProvider.AddLimits<AddonManager.RemoveAddonModule>(additionLimits)
                .AddLimits<AddonManager.AddAddonModule>(additionLimits)
                .AddLimit<AddonManager.ListAddonModule>(standartLimit)
                .AddLimit<AddonManager.ManageAddonModule>(standartLimit);
        }

        return new InteractionCommandBuilder<AddonManager>(ApplicationCommandType.SlashCommand) // TODO : Localization is not added to group commands yet.
            .WithName("addons")
            .AllowPrivateChannels(false)
            .WithDescription("All the addon related commands are here")
            .WithPermissions(Permissions.ManageGuild)
            .AddSubcommand<AddAddonModule>(addBuilder => addBuilder.WithName("add")
                .WithDescription("Adds addon to the guild")
                .AddOption(opb => opb.WithName("addon")
                    .WithType<string>()
                    .WithMetadata(ApplicationCommandOptionType.String, mpb => mpb.WithName("addon")
                    .WithDescription("Addon to add"))))
            .AddSubcommand<ListAddonModule>(listBuilder => listBuilder.WithName("list")
                .WithDescription("Lists the module registered to the guild"))
            .AddSubcommand<ManageAddonModule>(manageBuilder => manageBuilder.WithName("manage")
                .WithDescription("Adds addon to the guild")
                .AddOption(opb => opb.WithName("addon")
                    .WithDefaultValue(null)
                    .WithType<string>()
                    .WithMetadata(ApplicationCommandOptionType.String, mpb => mpb.WithName("addon")
                    .WithDescription("Addon to manage"))))
            .AddSubcommand<RemoveAddonModule>(removeBuilder => removeBuilder.WithName("remove")
                .WithDescription("Adds addon to the guild")
                .AddOption(opb => opb.WithName("addon")
                    .WithDefaultValue(null)
                    .WithType<string>()
                    .WithMetadata(ApplicationCommandOptionType.String, mpb => mpb.WithName("addon")
                        .WithDescription("Addon to manage"))));
    }
}