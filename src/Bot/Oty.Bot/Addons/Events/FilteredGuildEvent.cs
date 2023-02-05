namespace Oty.Bot.Addons.Events;

public class FilteredGuildEvent<TSender, TArgs> : IAsyncEventHandler<TSender, TArgs>
    where TArgs : AsyncEventArgs
{
    private readonly IGuildGetterExpressionCache _expressionCache;

    public FilteredGuildEvent(IGuildGetterExpressionCache expressionCache)
    {
        this._expressionCache = expressionCache;
    }

    public async Task ExecuteAsync(TSender sender, TArgs e)
    {

    }
}