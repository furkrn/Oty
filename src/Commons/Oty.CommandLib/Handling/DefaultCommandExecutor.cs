namespace Oty.CommandLib.Handling;

[PublicAPI]
public class DefaultCommandExecutor : ICommandExecutor
{
    private readonly ConcurrentDictionary<Type, Execution> _expressionCache = new();

    private bool _disposed;

    [PublicAPI]
    protected IOtyCommandsExtension? Extension { get; private set; }

    protected IReadOnlyDictionary<Type, Execution> ExpressionCache => this._expressionCache; // TODO: Document required...

    /// <inheritdoc/>
    [PublicAPI]
    public void Configure(IOtyCommandsExtension extension)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandExecutor));

        if (this.Extension is not null)
        {
            throw new InvalidOperationException("Executor is already configured.");
        }

        this.Extension = extension;

        extension.CommandAdded += this.CommandAdded;
        extension.CommandUpdated += this.CommandUpdated;
        extension.CommandRemoved += this.CommandRemoved;

        foreach (var command in extension.RegisteredCommands)
        {
            this.AddExpression(command);
        }

        this.Configure();
    }

    /// <inheritdoc/>
    [PublicAPI]
    public virtual bool CanExecute(BaseCommandMetadata metadata, [MaybeNullWhen(false)] out string? reason)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandExecutor));

        if (metadata is IGroupableMetadata<BaseCommandMetadata> groupable && groupable.Groups?.Count != 0)
        {
            if (!this.CanExecute(metadata))
            {
                reason = "Module must contain an constructor with only context of it";
                return false;
            }

            var failedModules = groupable.Groups?.SelectMany(sc => EnumerableExtensions.Traverse(sc, sb => (sb as IGroupableMetadata<BaseCommandMetadata>)?.Groups))
                .Select(m => m.Subcommand)
                .Where(c => !this.CanExecute(c))
                .ToArray();

            bool canExecute = failedModules?.Length == 0;

            reason = !canExecute ? $"Groups {failedModules?.Select(c => c.Name)} is not suitable" : null;
            return canExecute;
        }
        else
        {
            bool canExecute = this.CanExecute(metadata);

            reason = !canExecute ? "Module must contain an constructor with only context of it." : null;
            return canExecute;
        }
    }

    /// <inheritdoc/>
    [PublicAPI]
    public virtual async Task<Optional<CommandResult>> ExecuteAsync(IReceiverResult<DiscordEventArgs, BaseCommandMetadata, BaseCommandContext> context)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandExecutor));

        if (!this._expressionCache.TryGetValue(context.CommandMetadata.ModuleType, out var execution))
        {
            return Optional.FromNoValue<CommandResult>();
        }

        CommandResult result;
        try
        {
            var commandInstance = execution.InstanceFunc(context.CommandContext);

            bool canExecute = await commandInstance.BeforeExecutionAsync().ConfigureAwait(false);

            if (!canExecute)
            {
                throw new CheckFailedException(context.CommandMetadata, "Command checks are failed!");
            }

            await commandInstance.ExecuteAsync(context.MetadataOptions).ConfigureAwait(false);

            await commandInstance.AfterExecutionAsync().ConfigureAwait(false);

            result = CommandResult.FromSuccess(context.CommandMetadata, context.EventArgs);
        }
        catch (Exception ex)
        {
            result = CommandResult.FromFail(context.CommandMetadata, context.EventArgs, ex);
        }

        return result;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }


    [PublicAPI]
    protected virtual void Configure()
    {
    }

    [PublicAPI]
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            this._expressionCache.Clear();

            if (disposing)
            {
                if (this.Extension is not null)
                {
                    this.Extension!.CommandAdded -= this.CommandAdded;
                    this.Extension.CommandUpdated -= this.CommandUpdated;
                    this.Extension.CommandRemoved -= this.CommandRemoved;
                }
            }

            this.Extension = null;

            this._disposed = true;
        }
    }

    private Task CommandAdded(IOtyCommandsExtension sender, AddedCommandEventArgs e)
    {
        this.AddExpression(e.AddedCommand);

        return Task.CompletedTask;
    }

    private Task CommandUpdated(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        if (e.OldCommand.ModuleType == e.NewCommand.ModuleType)
        {
            return Task.CompletedTask;
        }

        if (this._expressionCache.TryGetValue(e.OldCommand.ModuleType, out var execution))
        {
            execution.DecreaseCommandCount();
            if (execution.Count is 0)
            {
                this._expressionCache.Remove(e.OldCommand.ModuleType, out _);
            }
        }

        this.AddExpression(e.NewCommand);

        return Task.CompletedTask;
    }

    private Task CommandRemoved(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        if (this._expressionCache.TryGetValue(e.RemovedCommand.ModuleType, out var execution))
        {
            execution.DecreaseCommandCount();
            if (execution.Count is 0)
            {
                this._expressionCache.Remove(e.RemovedCommand.ModuleType, out _);
            }
        }

        return Task.CompletedTask;
    }

    private void AddExpression(BaseCommandMetadata metadata)
    {
        var newContextType = this.Extension!.Receivers.Single(kp => kp.Key.CommandType == metadata.GetType())
            .Key
            .ContextType;

        var groups = EnumerableExtensions.Traverse(metadata, m => (m as IGroupableMetadata<BaseCommandMetadata>)?.Groups?.Select(c => c.Subcommand))
            .ToArray();

        foreach (var group in groups)
        {
            var func = CreateInstanceExpression(group.ModuleType, newContextType);
            this._expressionCache.AddOrUpdate(group.ModuleType, new Execution(func.Value), (_, e) =>  e.IncreaseCommandCount());
        }

        Optional<Func<BaseCommandContext, BaseCommandModule>> CreateInstanceExpression(Type moduleType, Type contextType)
        {
            var constructor = moduleType.GetConstructor(new[] { contextType });

            if (constructor is null)
            {
                return Optional.FromNoValue<Func<BaseCommandContext, BaseCommandModule>>();
            }

            var parameterExpression = Expression.Parameter(typeof(BaseCommandContext));
            var contextConvertExpression = Expression.Convert(parameterExpression, contextType);
            var newExpression = Expression.New(constructor, contextConvertExpression);
            var instanceConvertExpression = Expression.Convert(newExpression, typeof(BaseCommandModule));

            var blockExpression = Expression.Block(parameterExpression, contextConvertExpression, newExpression, instanceConvertExpression);

            return Expression.Lambda<Func<BaseCommandContext, BaseCommandModule>>(blockExpression, parameterExpression)
                .Compile();
        }
    }

    private bool CanExecute(BaseCommandMetadata metadata)
    {
        var contextType = this.Extension!.Receivers.FirstOrDefault(r => r.Key.CommandType == metadata.GetType())
                .Key
                .ContextType;

        var methods = metadata.ModuleType.GetMethods()
            .Where(m => m.GetCustomAttribute<TargetCommandMethodAttribute>() is not null)
            .ToArray();

        return metadata.ModuleType.GetConstructor(new[] { contextType }) is not null || (methods.Length == 1 && methods[0].ReturnType == typeof(Task));
    }

    protected sealed class Execution // TODO : Document required...
    {
        private int _count;

        public Execution(Func<BaseCommandContext, BaseCommandModule> instanceFunc)
        {
            this.InstanceFunc = instanceFunc ?? throw new ArgumentNullException(nameof(instanceFunc));
        }

        public Func<BaseCommandContext, BaseCommandModule> InstanceFunc { get; }

        public int Count => this._count;

        public Execution IncreaseCommandCount()
        {
            Interlocked.Increment(ref this._count);

            return this;
        }

        public Execution DecreaseCommandCount()
        {
            Interlocked.Decrement(ref this._count);

            return this;
        }
    }
}