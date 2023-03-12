namespace Oty.Interactivity.Entities;

public sealed class PaginationButtons
{
    public required DiscordButtonComponent FirstPageButton { get; init; }

    public required DiscordButtonComponent BackPageButton { get; init; }

    public DiscordButtonComponent? MiddleButton { get; init; }

    public required DiscordButtonComponent NextPageButton { get; init; }

    public required DiscordButtonComponent LastPageButton { get; init; }

    public IEnumerable<DiscordButtonComponent> GetButtonsAsCollection()
    {
        yield return this.FirstPageButton;
        yield return this.BackPageButton;
        if (this.MiddleButton is not null)
        {
            yield return this.MiddleButton;
        }
        yield return this.NextPageButton;
        yield return this.LastPageButton;
    }

    public IEnumerable<DiscordButtonComponent> GetButtonsAsCollectionWithCondition(uint current, uint maxValue)
    {
        if (current == 0)
        {
            yield return new DiscordButtonComponent(this.FirstPageButton).Disable();
            yield return new DiscordButtonComponent(this.BackPageButton).Disable();
            if (this.MiddleButton is not null)
            {
                yield return this.MiddleButton;
            }
            yield return new DiscordButtonComponent(this.NextPageButton).Enable();
            yield return new DiscordButtonComponent(this.LastPageButton).Enable();
        }
        else if (current < maxValue)
        {
            yield return new DiscordButtonComponent(this.FirstPageButton).Enable();
            yield return new DiscordButtonComponent(this.BackPageButton).Enable();
            if (this.MiddleButton is not null)
            {
                yield return this.MiddleButton;
            }
            yield return new DiscordButtonComponent(this.NextPageButton).Enable();
            yield return new DiscordButtonComponent(this.LastPageButton).Enable();
        }
        else
        {
            yield return new DiscordButtonComponent(this.FirstPageButton).Enable();
            yield return new DiscordButtonComponent(this.BackPageButton).Enable();
            if (this.MiddleButton is not null)
            {
                yield return this.MiddleButton;
            }
            yield return new DiscordButtonComponent(this.NextPageButton).Disable();
            yield return new DiscordButtonComponent(this.LastPageButton).Disable();
        }
    }

    public IEnumerable<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>> GetAsCommands(Paginator paginator)
    {
        ArgumentNullException.ThrowIfNull(paginator, nameof(paginator));

        yield return new((ComponentInteractivityRequest.ComponentKey)this.FirstPageButton, new(this.FirstPageButton.CustomId, this.FirstPageButton.Type, paginator.PaginateToFirstAsync));
        yield return new((ComponentInteractivityRequest.ComponentKey)this.BackPageButton, new(this.BackPageButton.CustomId, this.BackPageButton.Type, paginator.PaginateRewindAsync));
        yield return new((ComponentInteractivityRequest.ComponentKey)this.NextPageButton, new(this.NextPageButton.CustomId, this.NextPageButton.Type, paginator.PaginateForwardAsync));
        yield return new((ComponentInteractivityRequest.ComponentKey)this.LastPageButton, new(this.LastPageButton.CustomId, this.LastPageButton.Type, paginator.PaginateToLastAsync));
    }
}