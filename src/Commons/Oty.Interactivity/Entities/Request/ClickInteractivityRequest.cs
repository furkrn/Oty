using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace Oty.Interactivity.Entities;

public sealed class ClickInteractivityRequest : InteractivityRequest
{
    internal ClickInteractivityRequest(ClickInteractivityRequestBuilder builder)
    {
        this.TargetInteractionId = builder.TargetInteractionId;
        this.TargetUser = builder.TargetUser;
        this.TargetMessage = builder.TargetMessage ?? throw new ArgumentNullException(nameof(builder));
        this.IsRepeative = builder.IsRepeative;
        this.RepeativeInteractionExecutionTask = builder.RepeativeExecutionTask;
    }

    public bool IsRepeative { get; }

    public Func<ComponentInteractionCreateEventArgs, Task> RepeativeInteractionExecutionTask { get; }

    public string TargetInteractionId { get; }

    public DiscordMessage TargetMessage { get; }

    public DiscordUser TargetUser { get; }
}