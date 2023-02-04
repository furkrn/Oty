namespace Oty.Bot.Infastructure;

public interface IAsyncEventHandler
{
}

public interface IAsyncEventHandler<in TSender, in TArgs> : IAsyncEventHandler
    where TArgs : AsyncEventArgs
{
    Task ExecuteAsync(TSender sender, TArgs e);
}