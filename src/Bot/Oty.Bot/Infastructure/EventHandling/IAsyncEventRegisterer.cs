namespace Oty.Bot.Infastructure;

public interface IAsyncEventRegisterer
{
    IDisposable RegisterEvents<TSender>(TSender instance);
}