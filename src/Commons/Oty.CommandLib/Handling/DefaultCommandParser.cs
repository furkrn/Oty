namespace Oty.CommandLib.Handling;

public sealed class DefaultCommandParser : ICommandParser
{
    private readonly List<IParameterTypeParser> _parserCollection = new();

    private readonly ConcurrentDictionary<Type, (int Count, Delegate? Delegate)> _methods = new();

    private bool _disposed;

    private IOtyCommandsExtension? _extension;

    /// <inheritdoc/>
    public IReadOnlyList<IParameterTypeParser> RegisteredParsers
        => this._parserCollection;

    /// <inheritdoc/>
    [PublicAPI]
    public void Configure(IOtyCommandsExtension extension)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandParser));

        if (this._extension is not null)
        {
            throw new InvalidOperationException("Parser is already configured.");
        }

        extension.CommandAdded += this.OnCommandRegister;
        extension.CommandUpdated += this.OnCommandUpdate;
        extension.CommandRemoved += this.OnCommandRemoval;
        this._extension = extension;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._parserCollection.Clear();
        this._methods.Clear();

        this._disposed = true;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public void AddParser(IParameterTypeParser parser)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandParser));
        ArgumentNullException.ThrowIfNull(parser, nameof(parser));

        this._parserCollection.Add(parser);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public void RemoveParser(IParameterTypeParser parser)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandParser));
        ArgumentNullException.ThrowIfNull(parser, nameof(parser));

        this._parserCollection.Remove(parser);
    }

    // resharper disable once VariableHidesOuterVariable
    
    /// <inheritdoc/>
    [PublicAPI]
    public async Task ParseAndExecuteCommandAsync<TContext>(BaseCommandModule<TContext> moduleInstance, IEnumerable<KeyValuePair<IMetadataOption, object?>>? metadataOptionValues)
        where TContext : BaseCommandContext
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(DefaultCommandParser));
        ArgumentNullException.ThrowIfNull(moduleInstance, nameof(moduleInstance));
        metadataOptionValues ??= Enumerable.Empty<KeyValuePair<IMetadataOption, object?>>();

        if (moduleInstance.Context.Command is not ICommandArgumentsAvaliable<ICommandOption> options || options.Options?.Count == 0)
        {
            return;
        }

        if (!this._methods.TryGetValue(moduleInstance.Context.Command.ModuleType, out var execution) || execution.Delegate is null)
        {
            return;
        }

        var receivedEvent = (moduleInstance as IEventContext<DiscordEventArgs>)?.Event;

        var commandOptions = metadataOptionValues.GroupBy(m => m.Key.TargetOption)
            .Select(c => (c.Select(c => c.Value).ToArray(), c.Key));

        var parsedResultArray = new object?[options.Options!.Count + 1];
        parsedResultArray[0] = moduleInstance;

        int current = 1;
        foreach (var (values, option) in commandOptions) // TODO : Default value is ignored on this.
        {
            var parserContext = receivedEvent is null
                ? new TypeParserContext(this._extension!.Client, option, values)
                : new TypeParserContext<DiscordEventArgs>(this._extension!.Client, option, values, receivedEvent);

            parsedResultArray[current] = await this.ParseOptionAsync(parserContext).ConfigureAwait(false);

            current++;
        }

        var task = (Task?)execution.Delegate!.DynamicInvoke(parsedResultArray);

        await (task ?? Task.CompletedTask).ConfigureAwait(false);
    }

    private Task OnCommandRegister(IOtyCommandsExtension sender, AddedCommandEventArgs e)
    {
        if (this._methods.TryGetValue(e.AddedCommand.ModuleType, out _))
        {
            return Task.CompletedTask;
        }

        var methods = e.AddedCommand.ModuleType.GetMethods()
            .Where(m => m.GetCustomAttribute<TargetCommandMethodAttribute>() is not null)
            .ToArray();

        if (methods.Length == 1)
        {
            var methodDelegate = CreateDelegate(methods[0]);
            this._methods.AddOrUpdate(e.AddedCommand.ModuleType, (0, methodDelegate), (_, t) => (t.Count++, t.Delegate)); 
        }

        return Task.CompletedTask;
    }

    private Task OnCommandUpdate(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        if (e.NewCommand.ModuleType == e.OldCommand.ModuleType ||
            this._methods.TryGetValue(e.NewCommand.ModuleType, out _))
        {
            return Task.CompletedTask;
        }

        var methods = e.NewCommand.ModuleType.GetMethods()
            .Where(m => m.GetCustomAttribute<TargetCommandMethodAttribute>() is not null)
            .ToArray();

        if (methods.Length == 1)
        {
            var methodDelegate = CreateDelegate(methods[0]);
            this._methods.AddOrUpdate(e.NewCommand.ModuleType, (0, methodDelegate), (_, t) => (t.Count++, t.Delegate));
        }

        return Task.CompletedTask;
    }

    private Task OnCommandRemoval(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        if (this._methods.TryGetValue(e.RemovedCommand.ModuleType, out var tuple))
        {
            var count = --tuple.Count;

            if (count == 0)
            {
                this._methods.Remove(e.RemovedCommand.ModuleType, out _);
            }
        }

        return Task.CompletedTask;
    }

    private async Task<object?> ParseOptionAsync(ITypeParserContext context)
    {
        var parsers = this._parserCollection.Where(c => c.CanConvert(context));

        if (context.Values.Length > 1)
        {
            foreach (var parser in parsers)
            {
                var parsedResult = await parser.ConvertValueAsync(context);

                if (parsedResult.HasValue)
                {
                    return parsedResult.Value;
                }
            }
        }
        else if (context.Values.Length == 1)
        {
            var value = context.Values[0];

            if (context.Option.Type == value?.GetType())
            {
                return value;
            }
        }
        else
        {
            if (context.Option.DefaultValue.HasValue)
            {
                return context.Option.DefaultValue.Value;
            }
        }

        throw new InvalidOperationException($"No suitable parser registered for {context.Option.Type}");
    }

    // resharper disable once CoVariantArrayConversion

    private static Delegate CreateDelegate(MethodInfo method)
    {
        var instanceParameter = Expression.Parameter(method.DeclaringType!, "moduleInstance");
        var parameters = method.GetParameters()
            .Select(e => Expression.Parameter(e.ParameterType, e.Name))
            .ToArray();
        
        var methodCall = Expression.Call(instanceParameter, method, parameters);

        var parameterList = new List<ParameterExpression>()
        {
            instanceParameter,
        };
        parameterList.AddRange(parameters);

        return Expression.Lambda(methodCall, parameterList)
            .Compile();
    }
}