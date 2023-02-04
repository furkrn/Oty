using DSharpPlus;
using DSharpPlus.EventArgs;
using Oty.Interactivity.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Oty.Interactivity.Handlers;

internal sealed class ComponentInteractivityHandler : InteractivityHandler<ComponentInteractivityRequest, ComponentInteractionCreateEventArgs>, IDisposable
{
    public ComponentInteractivityHandler(DiscordClient client) : base(client)
    {
        this.Client.ComponentInteractionCreated += this.ComponentInteractionHandler;
    }

    public void Dispose()
    {
        this.Requests.Clear();
        this.Client.ComponentInteractionCreated -= this.ComponentInteractionHandler;
    }

    private Task ComponentInteractionHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var request = this.Requests.Keys.FirstOrDefault(c => c.TargetMessage == e.Message);

        if (request == null)
        {
            return Task.CompletedTask;
        }

        if (request.TargetUser != null && request.TargetUser != e.User)
        {
            return Task.CompletedTask;
        }

        if (!request.TargetComponents.TryGetValue(e.Interaction.Data, out var invoker))
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async() => await (invoker?.Caller?.Invoke(e) ?? Task.CompletedTask).ConfigureAwait(false));

        if (!this.Requests.TryGetValue(request, out var requestResults))
        {
            return Task.CompletedTask;
        }

        requestResults.DiscordEvent = e;
        requestResults.IsExecuted = true;
        requestResults.ExecutionCount++;
        requestResults.LastExecutedInteractionName = e.Interaction.Data.CustomId;

        if (!request.IsRepeative)
        {
            request.TimedOutTaskCompletionSource.SetResult(false);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}