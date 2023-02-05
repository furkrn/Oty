namespace Oty.Bot.Addons.Events;

public class FilteredGuildEvent<TSender, TArgs> : IAsyncEventHandler<TSender, TArgs>
    where TArgs : AsyncEventArgs
{
    private readonly IGuildGetterExpressionCache _expressionCache;

    public FilteredGuildEvent(IGuildGetterExpressionCache expressionCache)
    {
        this._expressionCache = expressionCache;
    }

    public Task ExecuteAsync(TSender sender, TArgs e)
    {
        throw new NotImplementedException();

        // TODO : Publish to specific addons later.
    }
}