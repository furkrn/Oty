namespace Oty.Bot.Infastructure;

// TODO : Support for addons...
public class LimitedExecutor : DefaultCommandExecutor
{
    private readonly ConcurrentDictionary<BaseCommandMetadata, Dictionary<ILimitation, LimitationResult?>> _currentLimits = new();

    private readonly ConcurrentDictionary<Type, HashSet<LimitationInfo>> _metadataProvidedLimits = new();

    private bool _disposed;

    private IStringLocalizer<LimitedExecutor>? _localizer;

    public override async Task<Optional<CommandResult>> ExecuteAsync(IReceiverResult<DiscordEventArgs, BaseCommandMetadata, BaseCommandContext> context)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(LimitedExecutor));

        this._localizer ??= context.CommandContext.RegisteredServices?.GetRequiredService<IStringLocalizer<LimitedExecutor>>();

        if (!this.ExpressionCache.TryGetValue(context.CommandMetadata.ModuleType, out _))
        {
            return Optional.FromNoValue<CommandResult>();
        }

        if (context.CommandContext is not BaseInteractionCommandContext commandContext ||
            !this._metadataProvidedLimits.TryGetValue(context.CommandMetadata.ModuleType, out var limitMetadata))
        {
            return await base.ExecuteAsync(context);
        }

        var limitDictionary = this._currentLimits.GetOrAdd(context.CommandMetadata, _ => new());

        if (!AllLocationsMatches(commandContext, limitMetadata, limitDictionary.Keys, out var usedLimitation))
        {
            var firstLimitInfo = limitMetadata.First();
            usedLimitation = new Limitation(commandContext.Command, GetLocation(commandContext, firstLimitInfo.Type),
            firstLimitInfo.MaximumUses, firstLimitInfo.LimitationTime);

            ILimitation limitation = usedLimitation;
            foreach (var limit in limitMetadata.Take(Range.StartAt(1)))
            {
                limitation = limitation.SetNext(new Limitation(commandContext.Command, GetLocation(commandContext, limit.Type),
                    limit.MaximumUses, limit.LimitationTime));
            }

            limitDictionary.Add(usedLimitation, null);
        }

        var lastLimitResult = limitDictionary[usedLimitation!];

        var now = DateTimeOffset.Now;
        var limitBuilder = new LimitationResultBuilder(usedLimitation!, now);

        await usedLimitation!.IncreaseAsync(limitBuilder);

        LimitationResult newLimitResult = limitBuilder;
        if (!HasNotExceedded(lastLimitResult, newLimitResult))
        {
            CultureInfo.CurrentCulture = commandContext.GetUserCultureInfo();

            var timeLeft = now.Subtract(newLimitResult.Expiration)
                .Humanize(1, CultureInfo.CurrentCulture, TimeUnit.Day, TimeUnit.Second, " ");

            string messageContent = this._localizer!["LimitedMessageContent", TranslationFindingType.WithOnlyTextContext, timeLeft];

            return Optional.FromValue(CommandResult.FromFail(context.CommandMetadata, context.EventArgs, new LimitedException(messageContent)));
        }

        var commandResult = await base.ExecuteAsync(context);

        if (IsFailed())
        {
            await newLimitResult.RevertChangesAsync();
        }
        else
        {
            limitDictionary[usedLimitation] = newLimitResult;
        }

        return commandResult;

        static bool HasNotExceedded(LimitationResult? lastLimitResult, LimitationResult newResult)
            => !lastLimitResult.HasValue || (lastLimitResult.Value.Exceedded is false && lastLimitResult.Value.Exceedded == newResult.Exceedded);

        bool IsFailed()
            => !commandResult.HasValue || (!commandResult.Value.IsExecuted && commandResult.Value.Exception is UsersFaultException);
    }

    protected override void Configure()
    {
        this.Extension!.CommandAdded += this.OnCommandRegistration;
        this.Extension.CommandUpdated += this.OnCommandUpdate;
        this.Extension.CommandRemoved += this.OnCommandRemove;
    }

    protected override void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing && this.Extension is not null)
            {
                this.Extension.CommandAdded -= this.OnCommandRegistration;
                this.Extension.CommandUpdated -= this.OnCommandUpdate;
                this.Extension.CommandRemoved -= this.OnCommandRemove;
            }

            this._disposed = true;
        }

        base.Dispose(disposing);
    }

    private static bool AllLocationsMatches(BaseInteractionCommandContext context, HashSet<LimitationInfo> metadataProvider, IReadOnlyCollection<ILimitation> limitations, [MaybeNullWhen(false)] out ILimitation? specifiedLimitation)
    {
        if (limitations.Count is 0)
        {
            specifiedLimitation = null;
            return false;
        }

        var firstLocationType = metadataProvider.First().Type;

        var firstLocation = GetLocation(context, firstLocationType);
        var limitation = limitations.FirstOrDefault(l => l.Location.Equals(firstLocation));

        if (limitation is not null)
        {
            var chainedLimitation = limitation.Next;

            while (chainedLimitation is not null)
            {
                if (!chainedLimitation.Location.Equals(GetLocation(context, chainedLimitation.Location.Type)))
                {
                    specifiedLimitation = null;
                    return false;
                }

                chainedLimitation = chainedLimitation.Next;
            }
        }
        else
        {
            specifiedLimitation = null;
            return false;
        }

        specifiedLimitation = limitation;
        return true;
    }

    private static ILimitationLocation GetLocation(BaseInteractionCommandContext commandContext, LimitationTypes type)
    {
        SnowflakeObject snowflake = type switch
        {
            LimitationTypes.ChannelWise => commandContext.Channel,
            LimitationTypes.UserWise => commandContext.User,
            _ => commandContext.Guild,
        };

        return new SnowflakeLimitationLocation(snowflake, type);
    }

    private Task OnCommandRegistration(IOtyCommandsExtension _, AddedCommandEventArgs e)
    {
        if (e.MetadataProvider is LimitedCommandMetadataProvider limitedProvider && limitedProvider.SpecifiedLimits.Count > 0)
        {
            foreach (var (type, limits) in limitedProvider.SpecifiedLimits)
            {
                this._metadataProvidedLimits.TryAdd(type, limits);
            }
        }

        return Task.CompletedTask;
    }

    private Task OnCommandRemove(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        this.Remove(e.RemovedCommand);

        return Task.CompletedTask;
    }

    private Task OnCommandUpdate(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        this.Remove(e.OldCommand);
        if (e.MetadataProvider is LimitedCommandMetadataProvider limitedProvider && limitedProvider.SpecifiedLimits.Count > 0)
        {
            foreach (var (type, limits) in limitedProvider.SpecifiedLimits)
            {
                this._metadataProvidedLimits.TryAdd(type, limits);
            }
        }

        return Task.CompletedTask;
    }

    private void Remove(BaseCommandMetadata metadata)
    {
        this._metadataProvidedLimits.TryRemove(metadata.ModuleType, out _);
        this._currentLimits.TryRemove(metadata, out _);
    }
}