using DSharpPlus.EventArgs;
using System.Collections.Generic;

namespace Oty.Interactivity;
public interface IInteractivityInteraction
{
    public bool IsExecuted { get; }

    public string LastExecutedInteractionName { get; }

    public int ExecutionCount { get; }

    public IReadOnlyDictionary<string, string> Entries { get; }
}

public readonly struct InteractivityResult<TEvent> : IInteractivityInteraction
    where TEvent : DiscordEventArgs
{
    internal InteractivityResult(TEvent result, bool timedOut, string executedInteractionName, bool isExecuted, int executionCount, IReadOnlyDictionary<string, string> entries)
    {
        this.EventResult = result;
        this.IsExecuted = isExecuted;
        this.IsTimedOut = timedOut;
        this.LastExecutedInteractionName = executedInteractionName;
        this.ExecutionCount = executionCount;
        this.Entries = entries;
    }

    internal InteractivityResult(TEvent result, bool timedOut, IInteractivityInteraction interaction)
    {
        this.EventResult = result;
        this.IsExecuted = interaction.IsExecuted;
        this.LastExecutedInteractionName = interaction.LastExecutedInteractionName;
        this.IsTimedOut = timedOut;
        this.ExecutionCount = interaction.ExecutionCount;
        this.Entries = interaction.Entries;
    }

    public TEvent EventResult { get; }

    public bool IsExecuted { get; }

    public string LastExecutedInteractionName { get; }

    public bool IsTimedOut { get; }

    public int ExecutionCount { get; }

    public IReadOnlyDictionary<string, string> Entries { get; }
}

public sealed class InteractivityInteractions : IInteractivityInteraction
{
    public DiscordEventArgs DiscordEvent { get; internal set; }

    public bool IsExecuted { get; internal set; }

    public string LastExecutedInteractionName { get; internal set; }

    public int ExecutionCount { get; internal set; }

    public IReadOnlyDictionary<string, string> Entries { get; internal set; }
}