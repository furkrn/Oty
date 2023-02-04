namespace Oty.CommandLib.Interactions;

internal sealed class OtyInteractionCommandExtension : IDisposable
{
    private readonly IOtyCommandsExtension _extension;

    private readonly InteractionCommandConfiguration _configuration;

    private bool _isClientReady;

    internal OtyInteractionCommandExtension(IOtyCommandsExtension extension, InteractionCommandConfiguration configuration)
    {
        if (configuration.PublishPublishables || configuration.PublishWhenClientReady)
        {
            extension.Client.Ready += this.ClientReady;
        }

        this._extension = extension;
        this._configuration = configuration;
    }

    private async Task RegisteredCommand(IOtyCommandsExtension sender, AddedCommandEventArgs e)
    {
        if (!this._configuration.PublishPublishables || e.AddedCommand is not IDiscordPublishable publishable)
        {
            return;
        }

        try
        {
            DiscordApplicationCommand convertedCommand = publishable.ToDiscordCommand();

            var applicationCommandTask = publishable.RegisteredGuildId.HasValue
                ? sender.Client.CreateGuildApplicationCommandAsync(publishable.RegisteredGuildId.Value, convertedCommand)
                : sender.Client.CreateGlobalApplicationCommandAsync(convertedCommand);

            var registeredCommand = await applicationCommandTask.ConfigureAwait(false);

            publishable.Id = registeredCommand.Id;
        }
        catch (Exception ex)
        {
            sender.Client.Logger.LogError("There was an error registering command {0} for {1} : \n {2}", publishable.Name, publishable.RegisteredGuildId?.ToString() ?? "globally", ex is BadRequestException badRequestException ? badRequestException.JsonMessage : ex.Message);
        }
    }

    private async Task UpdatedRegisteredCommand(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        if (!this._configuration.PublishPublishables ||
            (e.OldCommand is not IDiscordPublishable oldPublishable) || 
            (e.NewCommand is not IDiscordPublishable newPublishable))
        {
            return;
        }

        try
        {
            Action<ApplicationCommandEditModel> editModel = (edit) =>
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
            };

            var applicationCommandTask = oldPublishable.RegisteredGuildId.HasValue
                ? sender.Client.EditGuildApplicationCommandAsync(oldPublishable.RegisteredGuildId.Value, oldPublishable.Id, editModel)
                : sender.Client.EditGlobalApplicationCommandAsync(oldPublishable.Id, editModel);

            var updatedApplicationCommand = await applicationCommandTask.ConfigureAwait(false);

            newPublishable.Id = updatedApplicationCommand.Id;
        }
        catch (Exception ex)
        {
            sender.Client.Logger.LogError("There was a error while trying to update command {0} -> {1} that is registered in {2} : \n {3}", oldPublishable.Name, newPublishable.Name, oldPublishable.RegisteredGuildId?.ToString() ?? "all guilds", ex is BadRequestException badRequest ? badRequest.JsonMessage : ex.Message);
        }
    }

    private async Task RemovedCommand(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        if (!this._configuration.PublishPublishables || e.RemovedCommand is not IDiscordPublishable publishable)
        {
            return;
        }

        try
        {
            var task = publishable.RegisteredGuildId.HasValue 
                ? sender.Client.DeleteGuildApplicationCommandAsync(publishable.RegisteredGuildId.Value, publishable.Id)
                : sender.Client.DeleteGlobalApplicationCommandAsync(publishable.Id);

            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            sender.Client.Logger.LogError("There was a error for removal of command {0} that is registered in {1} : \n {2}", publishable.Name, publishable.RegisteredGuildId?.ToString() ?? "all guilds", ex is BadRequestException badRequest ? badRequest.JsonMessage : ex.Message);
        }
    }

    private async Task ClientReady(DiscordClient client, ReadyEventArgs e)
    {
        if (!this._configuration.PublishWhenClientReady)
        {
            return;
        }

        var guildPublishableDictionary = this._extension.RegisteredCommands
            .OfType<IDiscordPublishable>()
            .GroupBy(c => c.RegisteredGuildId ?? 0)
            .ToDictionary(d => d.Key);

        foreach ((ulong guildId, IEnumerable<IDiscordPublishable> publishables) in guildPublishableDictionary)
        {
            IEnumerable<DiscordApplicationCommand> registeredCommands;
            try
            {
                IEnumerable<DiscordApplicationCommand> convertedCommands = publishables.Select(p => p.ToDiscordCommand());

                var bulkOverwriteTask = guildId is 0
                    ? client.BulkOverwriteGuildApplicationCommandsAsync(guildId, convertedCommands)
                    : client.BulkOverwriteGlobalApplicationCommandsAsync(convertedCommands);

                registeredCommands = await bulkOverwriteTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                client.Logger.LogError("There was a error for registering for {0} : \n {1}", guildId.ToString() ?? "globally", ex is BadRequestException badRequest ? badRequest.JsonMessage : ex.Message);

                return;
            }

            foreach (var command in registeredCommands)
            {
                var moduleCommandEntity = publishables.First(c => c.Name == command.Name);

                moduleCommandEntity.Id = command.Id;
            }
        }

        this._extension.CommandAdded += this.RegisteredCommand;
        this._extension.CommandUpdated += this.UpdatedRegisteredCommand;
        this._extension.CommandRemoved += this.RemovedCommand;

        this._isClientReady = true;
    }

    public void Dispose()
    {
        if (this._isClientReady)
        {
            this._extension.CommandAdded -= this.RegisteredCommand;
            this._extension.CommandUpdated -= this.UpdatedRegisteredCommand;
            this._extension.CommandRemoved -= this.RemovedCommand;
        }

        this._extension.Client.Ready -= this.ClientReady;
    
    }
}

public static class InteractionCommandExtensions
{
    public static IDisposable InitializeInteractionCommands(this IOtyCommandsExtension extension, InteractionCommandConfiguration? configuration = null)
    {
        ArgumentNullException.ThrowIfNull(extension, nameof(extension));

        var slashReceiver = new SlashInteractionCommandReceiver();
        extension.AddReceiver(slashReceiver);
        extension.AddReceiver(new AutoCompleteCommandReceiver<SlashInteractionCommand, SlashInteractionContext>(slashReceiver));
        extension.AddReceiver(new PublishableCommandReceiver<ContextMenuInteractionCreateEventArgs, ContextMenuInteractionCommand, ContextMenuInteractionCommandContext>(
            (command, context) => new ContextMenuInteractionCommandContext(context.Client, context.Extension, command, context.EventArgs.Interaction, context.EventArgs, context.ServiceProvider)));

        return new OtyInteractionCommandExtension(extension, configuration ?? new());
    }
}