namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class OwnerEvalModule : BaseOwnerModule<ContextMenuInteractionCommandContext>, IMetadataCreatable
{
    public OwnerEvalModule(ContextMenuInteractionCommandContext context) : base(context)
    {
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        ulong specialGuildId = metadataProvider.Services?.GetRequiredService<IOptions<BotConfiguration>>()
            .Value
            .SpecialGuildId ?? 0;

        return new InteractionCommandBuilder<OwnerEvalModule>(ApplicationCommandType.MessageContextMenu)
            .WithName("Eval")
            .WithGuildId(specialGuildId);
    }

    public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        string code = this.Context.TargetMessage!.Content;

        int firstIndex = code.IndexOf("```cs", StringComparison.Ordinal) + 5;
        int lastIndex = code.LastIndexOf("```", StringComparison.Ordinal);

        if (firstIndex == -1 || lastIndex == -1)
        {
            UsersFaultException.ThrowFrom(new ArgumentException("Code should be wrapped on 'cs' code block."));
        }

        code = code[firstIndex..lastIndex];

        return EvaluateCodeAsync(this.Context, code);
    }

    private static async Task EvaluateCodeAsync(ContextMenuInteractionCommandContext context, string code)
    {
        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        DiscordEmbed embed;
        try
        {
            var evalEnvironment = new EvalEnvironment(context);
            var scriptOptions = ScriptOptions.Default
                         .AddImports("System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http", "System.Net.Http.Headers", "System.Reflection", "System.Text",
                         "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.Entities", "DSharpPlus.EventArgs", "DSharpPlus.Exceptions", "Microsoft.Extensions.DependencyInjection",
                         "Oty.Bot.Core", "Oty.CommandLib", "Oty.Bot.InternalModules", "Oty.Interactivity")
                         .AddReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));

            var script = CSharpScript.Create(code, scriptOptions, typeof(EvalEnvironment));
            script.Compile();

            var state = await script.RunAsync(evalEnvironment);

            embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.SpringGreen)
                .WithTitle("Successfully evaluted that code!")
                .WithDescription(state.ReturnValue != null ? $"Value returned {state.ReturnValue.GetType()} : {state.ReturnValue}" :
                    "No value returned!");
        }
        catch (Exception exception)
        {
            embed = new DiscordEmbedBuilder()
                .WithTitle("Failed to evalute that code!")
                .WithDescription($"Here's some exceptions for that shit : \n {exception.Message}")
                .WithColor(DiscordColor.Red);
        }

        var webhookMessage = new DiscordWebhookBuilder()
            .AddEmbed(embed);

        await context.EditResponseAsync(webhookMessage);
    }
}

public record EvalEnvironment(ContextMenuInteractionCommandContext Context)
{
    public DiscordChannel Channel => Context.Channel;

    public DiscordMember? Member => Context.Member;

    public DiscordGuild Guild => Context.Guild;

    public DiscordClient Client => Context.Client;

    public IServiceProvider? Services => Context.RegisteredServices;
}