namespace Oty.Bot.Infastructure;

public sealed class AsyncEventHandlerCollectionBuilder
{
    private readonly ConcurrentDictionary<(Type SenderType, Type EventArgsType), List<Type>> _eventHandlerTypes = new();

    public AsyncEventHandlerCollectionBuilder AddEventHandler<TSender, TEventArgs, TEventHandler>()
        where TEventArgs : AsyncEventArgs
        where TEventHandler : IAsyncEventHandler<TSender, TEventArgs>
    {
        var type = typeof(TEventHandler);

        this._eventHandlerTypes.AddOrUpdate((typeof(TSender), typeof(TEventArgs)), _ => new() { type }, (_, l) =>
        {
            l.Add(type);
            return l;
        });

        return this;
    }

    public IReadOnlyList<EventHandlerType> Build()
    {
        return this._eventHandlerTypes.Select(c => new EventHandlerType(c.Key.SenderType, c.Key.EventArgsType, c.Value))
            .ToList();
    }
}