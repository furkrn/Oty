namespace Oty.Bot.InternalModules;

[PublicAPI]
public abstract class BaseOwnerModule<TContext> : BaseCommandModule<TContext>
    where TContext : BaseInteractionCommandContext
{
    protected BaseOwnerModule(TContext context) : base(context)
    {
    }

    public override Task<bool> BeforeExecutionAsync()
    {
        return Task.FromResult(this.Context.Client.CurrentApplication.Owners.Contains(this.Context.User));
    }
}