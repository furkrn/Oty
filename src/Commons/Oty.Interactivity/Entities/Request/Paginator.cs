namespace Oty.Interactivity.Entities;

[PublicAPI]
public class Paginator : IPaginator
{
    [PublicAPI]
    public IReadOnlyList<InteractionPage> Pages { get; set; }

    protected uint Counter { get; set; }

    [UsedImplicitly]
    public virtual Task<InteractionPage> GetCurrentPageAsync()
    {
        return Task.FromResult(this.Pages[(int)this.Counter]);
    }

    [UsedImplicitly]
    public virtual Task PaginateForwardAsync(ComponentInteractionCreateEventArgs eventArgs)
    {
        if (this.Counter < this.Pages.Count - 1)
        {
            this.Counter++;
        }

        return this.SendPageAsync(eventArgs);
    }

    [UsedImplicitly]
    public virtual Task PaginateRewindAsync(ComponentInteractionCreateEventArgs eventArgs)
    {
        if (this.Counter > 0)
        {
            this.Counter--;
        }

        return this.SendPageAsync(eventArgs);
    }

    [UsedImplicitly]
    public virtual Task PaginateToFirstAsync(ComponentInteractionCreateEventArgs eventArgs)
    {
        this.Counter = 0;

        return this.SendPageAsync(eventArgs);
    }

    [UsedImplicitly]
    public virtual Task PaginateToLastAsync(ComponentInteractionCreateEventArgs eventArgs)
    {
        this.Counter = (uint)this.Pages.Count - 1;

        return this.SendPageAsync(eventArgs);
    }

    [UsedImplicitly]
    public virtual async Task SendPageAsync(ComponentInteractionCreateEventArgs eventArgs)
    {
        var currentPage = await this.GetCurrentPageAsync().ConfigureAwait(false);

        await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, currentPage).ConfigureAwait(false);
    }
}