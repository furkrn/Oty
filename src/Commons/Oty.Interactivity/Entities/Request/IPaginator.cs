namespace Oty.Interactivity.Entities;

public interface IPaginator
{
    IReadOnlyList<InteractionPage> Pages { get; set; }

    Task<InteractionPage> GetCurrentPageAsync();

    Task PaginateForwardAsync(ComponentInteractionCreateEventArgs eventArgs);

    Task PaginateRewindAsync(ComponentInteractionCreateEventArgs eventArgs);

    Task PaginateToFirstAsync(ComponentInteractionCreateEventArgs eventArgs);

    Task PaginateToLastAsync(ComponentInteractionCreateEventArgs eventArgs); 
}