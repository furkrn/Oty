namespace Oty.Bot.Infastructure;

public sealed class AsyncEventHandlerFactoryCollectionBuilder
{
    public AsyncEventHandlerFactoryCollectionBuilder(IServiceCollection serviceCollection)
    {
        this.ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
    }

    public IServiceCollection ServiceCollection { get; }

    public AsyncEventHandlerFactoryCollectionBuilder AddFactory<TSender, TArgs, THandler, TFactory>()
        where TArgs : AsyncEventArgs
        where THandler : IAsyncEventHandler<TSender, TArgs>
        where TFactory : class, IAsyncEventHandlerFactory<THandler>
    {
        this.ServiceCollection.AddSingleton<IAsyncEventHandlerFactory<THandler>, TFactory>();

        return this;
    }
}