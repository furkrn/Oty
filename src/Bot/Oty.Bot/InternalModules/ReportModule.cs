namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class ReportModule : BaseVerifiedCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    //private readonly IStringLocalizer<ReportModule>? _localizer;

    public ReportModule(SlashInteractionContext context) : base(context)
    {
        //this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<ReportModule>>();
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        if (metadataProvider is LimitedCommandMetadataProvider limitedMetadataProvider)
        {
            limitedMetadataProvider.AddLimit<ReportModule>(LimitationTypes.UserWise, TimeSpan.FromMinutes(1), 1);
        }

        return new InteractionCommandBuilder<ReportModule>(ApplicationCommandType.SlashCommand)
            .WithName("report")
            .WithDescription("Report Abuse")
            .WithPermissions(Permissions.UseApplicationCommands)
            .AddOption(moduleOptionBuilder => moduleOptionBuilder.WithName("module")
                .WithType<string>()
                .WithMetadata(ApplicationCommandOptionType.String, metadataBuilder => metadataBuilder.WithName("module")
                    .WithDescription("Uh! You thingy... Give it to me")
                    .WithAutoCompleteCommand(ModuleFinderAutoCompleteModule.ModuleName)
                    .LocalizeFrom<ReportModule>(metadataProvider, ln => ln["ReportModuleOption", TranslationFindingType.WithOnlyTextContext])))
            .LocalizeFrom(metadataProvider);
    }

    public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        if (this.TosValidatedInteraction is not null)
        {
            await this.Context.DeleteOriginalResponseAsync();
        }

        await base.ExecuteAsync(parameterCollection);
    }
}