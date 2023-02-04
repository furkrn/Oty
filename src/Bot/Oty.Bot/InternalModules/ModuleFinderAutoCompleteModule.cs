namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class ModuleFinderAutoCompleteModule : BaseAutoCompleteModule, IMetadataCreatable
{
    public const string ModuleName = "moduleFinder";

    public ModuleFinderAutoCompleteModule(AutoCompleteInteractionContext context) : base(context)
    {
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        return new InteractionCommandBuilder<ModuleFinderAutoCompleteModule>(ApplicationCommandType.AutoCompleteRequest)
            .WithName(ModuleName);
    }

    public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        var choice = new DiscordAutoCompleteChoice("sex", "seggs");

        this.Context.Choices = new[] { choice };
        return Task.CompletedTask;
    }
}