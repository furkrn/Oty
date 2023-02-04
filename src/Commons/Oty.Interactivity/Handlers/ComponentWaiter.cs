using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Oty.Interactivity.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Oty.Interactivity.Handlers;

internal sealed class ComponentWaiter : InteractivityHandler<ClickInteractivityRequest, ComponentInteractionCreateEventArgs>, IDisposable
{
    // resharper disable PossibleNullReferenceException

    private readonly DiscordInteractionResponseBuilder _noPermissionInteractionBuilder;

    public ComponentWaiter(DiscordClient client, OtyInteractivityConfiguration? configuration) : base(client)
    {
        this._noPermissionInteractionBuilder = configuration?.NoPermissionResponseBuilder ?? throw new ArgumentNullException(nameof(configuration));

        this.Client.ComponentInteractionCreated += this.ComponentInteractionCreatedHandling;
    }

    private async Task ComponentInteractionCreatedHandling(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var request = this.Requests.Keys.FirstOrDefault(request => request.TargetMessage == e.Message);

        if (request == null)
        {
            return;
        }

        string interactionId = e.Interaction?.Data?.CustomId;

        if (!string.IsNullOrWhiteSpace(request.TargetInteractionId) && request.TargetInteractionId != interactionId)
        {
            return;
        }

        if (request.TargetUser != null && request.TargetUser != e.Interaction.User)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, this._noPermissionInteractionBuilder).ConfigureAwait(false);

            return;
        }

        if (!this.Requests.TryGetValue(request, out var interaction))
        {
            return;
        }

        interaction.IsExecuted = true;
        interaction.ExecutionCount++;
        interaction.LastExecutedInteractionName = e.Interaction.Data.CustomId;
        interaction.DiscordEvent = e;

        await (request?.RepeativeInteractionExecutionTask?.Invoke(e) ?? Task.CompletedTask).ConfigureAwait(false);

        if (!request.IsRepeative)
        {
            request.TimedOutTaskCompletionSource.SetResult(false);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Requests.Clear();
        this.Client.ComponentInteractionCreated -= this.ComponentInteractionCreatedHandling;
    }

    // resharper restore all
}