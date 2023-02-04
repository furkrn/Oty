namespace Oty.Bot.Infastructure;

public class AsyncEventRegisterer : IAsyncEventRegisterer
{
    private readonly IServiceProvider _serviceProvider;

    private readonly AsyncEventHandlerOptions _handlerOptions;

    private readonly IAsyncEventHandlerFactory _defaultFactory;

    private readonly IAsyncEventRegistrationCreator _registrationCreator;

    private readonly IAsyncEventHandlerDelegateCreator _eventHandlerCreator;

    public AsyncEventRegisterer(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this._handlerOptions = serviceProvider.GetRequiredService<IOptions<AsyncEventHandlerOptions>>().Value;
        this._defaultFactory = serviceProvider.GetRequiredService<IAsyncEventHandlerFactory>();
        this._registrationCreator = serviceProvider.GetRequiredService<IAsyncEventRegistrationCreator>();
        this._eventHandlerCreator = serviceProvider.GetRequiredService<IAsyncEventHandlerDelegateCreator>();
    }

    public IDisposable RegisterEvents<TSender>(TSender instance)
    {
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));

        var unregisterDictionary = new Dictionary<RegistrationHandler, List<Delegate>>();

        foreach (var eventHandler in this._handlerOptions.RegisteredOptions.Where(c => c.SenderType == typeof(TSender)))
        {
            foreach (var handlerType in eventHandler.EventHandlerTypes)
            {
                var registerer = this._registrationCreator.CreateRegisterationDelegate(eventHandler.SenderType, eventHandler.EventArgsType, handlerType);
                var handlerInstance = this.CreateHandler(handlerType);
                var handler = this._eventHandlerCreator.CreateDelegate(handlerInstance, eventHandler.SenderType, eventHandler.EventArgsType);

                registerer(instance, handler);

                var unregisterer = this._registrationCreator.CreateUnregisterationDelegate(eventHandler.SenderType, eventHandler.EventArgsType, handlerType);

                CreateListOrAddToList(unregisterDictionary, unregisterer, handler);
            }
        }

        return new SenderUnregisterer(instance, unregisterDictionary);
    }

    private IAsyncEventHandler CreateHandler(Type type)
    {
        var handlerFactoryType = typeof(IAsyncEventHandlerFactory<>).MakeGenericType(type);

        var factory = this._serviceProvider.GetService(handlerFactoryType) as IAsyncEventHandlerFactory 
            ?? this._defaultFactory;

        return factory.Create(type);
    }

    private static void CreateListOrAddToList(Dictionary<RegistrationHandler, List<Delegate>> unregisterDictionary, RegistrationHandler unregisterer, Delegate handler)
    {
        if (!unregisterDictionary.TryGetValue(unregisterer, out var delegates))
        {
            delegates = new();
            unregisterDictionary.Add(unregisterer, delegates);
        }

        delegates.Add(handler);
    }

    private sealed class SenderUnregisterer : IDisposable
    {
        private readonly object _instance;

        private readonly IReadOnlyDictionary<RegistrationHandler, List<Delegate>> _unregisterDictionary;

        public SenderUnregisterer(object instance, IReadOnlyDictionary<RegistrationHandler, List<Delegate>> unregisterDictionary)
        {
            this._instance = instance;
            this._unregisterDictionary = unregisterDictionary;
        }

        public void Dispose()
        {
            foreach (var (unregisterer, handlers) in this._unregisterDictionary)
            {
                foreach (var handler in handlers)
                {
                    unregisterer(this._instance, handler);
                }
            }
        }
    }
}