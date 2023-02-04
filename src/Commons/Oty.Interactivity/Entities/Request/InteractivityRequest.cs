namespace Oty.Interactivity.Entities;

public abstract class InteractivityRequest
{
    protected InteractivityRequest()
    {
        this.TimedOutTaskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    internal TaskCompletionSource<bool> TimedOutTaskCompletionSource { get; }

    public Task FinishAsync()
    {
        this.TimedOutTaskCompletionSource.SetResult(false);
        
        return Task.CompletedTask;
    }
}