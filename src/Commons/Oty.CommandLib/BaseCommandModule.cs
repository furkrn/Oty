namespace Oty.CommandLib;

/// <summary>
/// A base class for handling a command.
/// </summary>
public abstract class BaseCommandModule
{
    private protected BaseCommandModule()
    {
    }

    /// <summary>
    /// Executes later any application command that is declared on the specified type.
    /// </summary>
    [PublicAPI]
    public virtual Task AfterExecutionAsync()
        => Task.CompletedTask;

    /// <summary>
    /// Indicates module can be executed before any application command that is declared on the specified type.
    /// </summary>
    /// <returns><see langword="true"/> if any command on the module can be executed, <see langword="false"/> any command on this module cannot be executed!</returns>
    [PublicAPI]    
    public virtual Task<bool> BeforeExecutionAsync()
        => Task.FromResult(true);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameterCollection">Unparsed dictionary of options.</param>
    public virtual Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
        => Task.CompletedTask;
}

/// <summary>
/// A base class for handling a command with context.
/// </summary>
[PublicAPI]
public abstract class BaseCommandModule<TContext> : BaseCommandModule
    where TContext : BaseCommandContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    [UsedImplicitly]
    protected BaseCommandModule(TContext context)
    {
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets the command context.
    /// </summary>
    [PublicAPI]
    public TContext Context { get; }

    /// <inheritdoc/>
    public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        return this.Context.Extension.CommandParser.ParseAndExecuteCommandAsync(this, parameterCollection);
    }
}