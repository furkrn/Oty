namespace Oty.Bot.Commands;

[PublicAPI]
public class CommandsRegisterer : ICommandsRegisterer
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<CommandsRegisterer> _logger;

    private readonly CommandsConfiguration _configuration;

    private readonly IServiceProvider _serviceProvider;

    private readonly ICheckCollection _checkCollection;

    private readonly ConcurrentDictionary<int, (IOtyCommandsExtension Extension, bool IsClientReady)> _registeredExtensions = new();

    public CommandsRegisterer(IServiceScopeFactory factory, ILogger<CommandsRegisterer> logger, IOptions<CommandsConfiguration> configuration,
        IServiceProvider serviceProvider, ICheckCollection checkCollection)
    {
        this._scopeFactory = factory;
        this._logger = logger;
        this._configuration = configuration.Value;
        this._serviceProvider = serviceProvider;
        this._checkCollection = checkCollection;
    }

    // resharper disable SuspiciousTypeConversion.Global
    // resharper disable PossibleMultipleEnumeratio

    public async Task RegisterCommandsAsync(IOtyCommandsExtension extension)
    {
        ArgumentNullException.ThrowIfNull(extension, nameof(extension));

        if (extension.Client is null)
        {
            throw new ArgumentException("Specified command extension must be registered to client.", nameof(extension));
        }

        this._registeredExtensions.TryAdd(extension.Client.ShardId, (extension, false));

        extension.Client.Ready += this.OnReadyClient;

        if (extension.Client.ShardId is 0)
        {
            extension.Client.Ready += this.OnReadyShard0Client;
        }

        await Task.WhenAll(this._configuration.RegisteredTypes
            .Select(RegisterModuleAsync));

        extension.CommandAdded += this.OnCommandRegistration;
        extension.CommandUpdated += this.OnCommandUpdate;
        extension.CommandRemoved += this.OnCommandRemove;

        Task<bool> RegisterModuleAsync(KeyValuePair<Type, ModuleMetadataHelper> keyValuePair)
        {
            var (moduleType, moduleHelper) = keyValuePair;

            return extension.RegisterCommandAsync(moduleType, moduleHelper.MetadataProviderFactory!(this._serviceProvider));
        }
    }

    private async Task CheckAndAddResultsAsync(DiscordClient client)
    {
        foreach (var (moduleType, metadataHelper) in this._configuration.RegisteredTypes
            .Where(c => c.Value.Check is not null))
        {
            bool? result = await metadataHelper.Check!.CheckAsync(client, this._serviceProvider);

            if (result.HasValue)
            {
                this._checkCollection.AddResult(moduleType, result.Value);
            }
        }
    }

    // resharper disable once AccessToDisposedClosure

    private Task OnReadyClient(DiscordClient sender, ReadyEventArgs e)
    {
        int shardId = sender.ShardId;
        var (extension, _) = this._registeredExtensions[shardId];

        var guilds = sender.Guilds;

        var extensionCommands = extension.RegisteredCommands
            .Where(c => this._configuration.RegisteredTypes.ContainsKey(c.ModuleType))
            .OfType<IDiscordPublishable>()
            .Where(c => guilds.TryGetValue(c.RegisteredGuildId.GetValueOrDefault(), out _))
            .ToArray();
        
        if (extensionCommands.Length is 0)
        {
            this._logger.LogInformation("Shard {0} has no internal commands to register to any guild.", sender.ShardId);

            return Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            await this.CheckAndAddResultsAsync(sender);

            await using var scope = this._scopeFactory.CreateAsyncScope();
            await using var repository = scope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();

            var publishableDictionary = extensionCommands.GroupBy(c => c.RegisteredGuildId ?? 0)
                .ToDictionary(d => d.Key);

            foreach ((ulong guildId, IEnumerable<IDiscordPublishable> commands) in publishableDictionary)
            {
                var tasks = await Task.WhenAll(commands.Select(c => repository.ContainsAsync(c)));

                IEnumerable<(string Name, ApplicationCommandType Type, ulong Id)> idCollection;
                if (tasks.All(d => d))
                {
                    idCollection = repository.GetCommandsBy(c => c.GuildId == guildId)
                        .Select(c => (c.Name, c.CommandType, c.Id));
                }
                else
                {
                    var publishableEntities = commands.Select(c => c.ToDiscordCommand());

                    idCollection = (await sender.BulkOverwriteGuildApplicationCommandsAsync(guildId, publishableEntities))
                        .Select(c => (c.Name, c.Type, c.Id));
                }

                foreach (var command in commands)
                {
                    var entity = idCollection.First(c => c.Name == command.Name && c.Type == command.CommandType);
                    command.Id = entity.Id;
                }

                await repository.BulkInsertOrUpdateOrDeleteFromAsync(commands);
            }

            await repository.UnitOfWork.SaveChangesAsync();
        });

        this._registeredExtensions[shardId] = (extension, true);

        return Task.CompletedTask;
    }

    private Task OnReadyShard0Client(DiscordClient sender, ReadyEventArgs e)
    {
        if (sender.ShardId is not 0)
        {
            return Task.CompletedTask;
        }

        var (extension, _) = this._registeredExtensions[0];

        var extensionCommands = extension.RegisteredCommands
            .Where(c => this._configuration.RegisteredTypes.ContainsKey(c.ModuleType))
            .OfType<IDiscordPublishable>()
            .Where(c => c.RegisteredGuildId is (0 or null))
            .ToArray();

        if (extensionCommands.Length is 0)
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            await using var scope = this._scopeFactory.CreateAsyncScope();
            await using var repository = scope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();

            int count = await repository.GetCountAsync(p => p.GuildId != null);
            var avaliableCommands = await sender.GetGlobalApplicationCommandsAsync();

            bool isCountLower = count != avaliableCommands.Count;

            if (isCountLower)
            {
                var publishableCommands = extensionCommands.Select(c => c.ToDiscordCommand());
                
                avaliableCommands = await sender.BulkOverwriteGlobalApplicationCommandsAsync(publishableCommands);
            }

            foreach (var command in this._registeredExtensions.SelectMany(kp => kp.Value.Extension.RegisteredCommands)
                .OfType<IDiscordPublishable>()
                .Where(c => c.RegisteredGuildId is (0 or null)))
            {
                var registeredCommand = avaliableCommands.FirstOrDefault(c => c.Name == command.Name 
                    && c.Type == command.CommandType);

                if (registeredCommand is null)
                {
                    this._logger.LogWarning("Command {0} with type {1} was registered into extension but not registered", command.Name, command.CommandType);

                    continue;
                }

                command.Id = registeredCommand.Id;
            }

            if (isCountLower)
            {
                await repository.BulkInsertOrUpdateOrDeleteFromAsync(extensionCommands);
                await repository.UnitOfWork.SaveChangesAsync();
            }
        });

        this._registeredExtensions[0] = (extension, true);

        return Task.CompletedTask;
    }

    private async Task OnCommandRegistration(IOtyCommandsExtension sender, AddedCommandEventArgs e)
    {
        var (_, isClientReady) = this._registeredExtensions[sender.Client.ShardId];

        if (!isClientReady ||
            e.AddedCommand is not IDiscordPublishable publishable)
        {
            return;
        }

        await using var scope = this._scopeFactory.CreateAsyncScope();
        await using var repository = scope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();

        var command = await repository.GetAsync(publishable.Name, publishable.CommandType);

        ulong id;
        if (command is null)
        {
            var pu = publishable.ToDiscordCommand();

            var moduleCreationTask = publishable.RegisteredGuildId is not (0 or null)
                ? sender.Client.CreateGuildApplicationCommandAsync(publishable.RegisteredGuildId.Value, pu)
                : sender.Client.CreateGlobalApplicationCommandAsync(pu);

            var createdCommand = await moduleCreationTask;
            id = createdCommand.Id;

            return;
        }
        else
        {
            id = command.Id;
        }

        publishable.Id = id;
    }

    private Task OnCommandUpdate(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        var (_, isClientReady) = this._registeredExtensions[sender.Client.ShardId];

        if (!isClientReady ||
            e.OldCommand is not IDiscordPublishable oldPublishable ||
            oldPublishable.RegisteredGuildId is (0 or null))
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async () => 
        {
            await using var scope = this._scopeFactory.CreateAsyncScope();
            await using var repository = scope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();

            if (!await repository.ContainsAsync(oldPublishable))
            {
                return;
            }

            if (e.NewCommand is not IDiscordPublishable newPublishable)
            {
                var task = oldPublishable.RegisteredGuildId is null
                    ? sender.Client.DeleteGlobalApplicationCommandAsync(oldPublishable.Id)
                    : sender.Client.DeleteGuildApplicationCommandAsync(oldPublishable.RegisteredGuildId.Value, oldPublishable.Id);
                
                await task;
            
                return;
            }

            void EditCommand(ApplicationCommandEditModel edit)
            {
                var newPublishableCommand = newPublishable.ToDiscordCommand();

                edit.AllowDMUsage = newPublishableCommand.AllowDMUsage!.Value;
                edit.DefaultMemberPermissions = newPublishableCommand.DefaultMemberPermissions;
                edit.DefaultPermission = newPublishable.DefaultPermission;
                edit.Description = newPublishableCommand.Description;
                edit.Name = newPublishableCommand.Name;

                if (newPublishableCommand.Options != null)
                {
                    edit.Options = Optional.FromValue(newPublishableCommand.Options);
                }
            }

            var updatedTask = newPublishable.RegisteredGuildId is null
                ? sender.Client.EditGlobalApplicationCommandAsync(oldPublishable.Id, EditCommand)
                : sender.Client.EditGuildApplicationCommandAsync(oldPublishable.RegisteredGuildId.Value, oldPublishable.Id, EditCommand);

            var updatedCommand = await updatedTask;

                await repository.UpdateAsync(oldPublishable.Id, _ => new InternalCommand()
                {
                    Name = updatedCommand.Name,
                    CommandType = updatedCommand.Type,
                    Id = updatedCommand.Id,
                    GuildId = oldPublishable.RegisteredGuildId,
                    HashCode = newPublishable.GetHashCode(),
                });

                await repository.UnitOfWork.SaveChangesAsync();
            });

        return Task.CompletedTask;
    }

    private Task OnCommandRemove(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        var (_, isClientReady) = this._registeredExtensions[sender.Client.ShardId];

        if (!isClientReady ||
            e.RemovedCommand is not IDiscordPublishable publishable ||
            publishable.Id is 0)
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            await using var scope = this._scopeFactory.CreateAsyncScope();
            await using var repository = scope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();

            if (!await repository.ContainsAsync(publishable))
            {
                return;
            }

            var task = publishable.RegisteredGuildId is null 
                ? sender.Client.DeleteGlobalApplicationCommandAsync(publishable.Id)
                : sender.Client.DeleteGuildApplicationCommandAsync(publishable.RegisteredGuildId.GetValueOrDefault(), publishable.Id);

            await task;
            await repository.TryRemove(publishable.Id);

            await repository.UnitOfWork.SaveChangesAsync();
        });

        return Task.CompletedTask;
    }
}