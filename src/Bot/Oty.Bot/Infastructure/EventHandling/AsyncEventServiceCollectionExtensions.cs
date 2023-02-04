namespace Oty.Bot.Infastructure;

public static class AsyncEventServiceCollectionExtensions
{
    public static AsyncEventHandlerFactoryCollectionBuilder ConfigureAsyncEventHandlers(this IServiceCollection serviceCollection, Func<AsyncEventHandlerCollectionBuilder, AsyncEventHandlerCollectionBuilder> builderFunc)
    {   
        ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
        ArgumentNullException.ThrowIfNull(builderFunc, nameof(builderFunc));

        var builder = new AsyncEventHandlerCollectionBuilder();
        builderFunc(builder);

        serviceCollection.TryAddSingleton<IAsyncEventRegisterer, AsyncEventRegisterer>();
        serviceCollection.TryAddSingleton<IAsyncEventHandlerFactory, DefaultAsyncEventHandlerFactory>();
        serviceCollection.TryAddSingleton<IAsyncEventRegistrationCreator, AsyncEventRegistrationCreator>();
        serviceCollection.TryAddSingleton<IAsyncEventHandlerDelegateCreator, AsyncEventHandlerDelegateCreator>();

        serviceCollection.Configure<AsyncEventHandlerOptions>(b => b.RegisteredOptions = builder.Build());

        return new AsyncEventHandlerFactoryCollectionBuilder(serviceCollection);
    }
}