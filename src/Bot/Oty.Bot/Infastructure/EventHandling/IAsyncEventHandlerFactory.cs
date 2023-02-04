namespace Oty.Bot.Infastructure;

public interface IAsyncEventHandlerFactory
{
    IAsyncEventHandler Create(Type type);
}

public interface IAsyncEventHandlerFactory<[UsedImplicitly] TEventHandler> : IAsyncEventHandlerFactory
    where TEventHandler : IAsyncEventHandler
{
}