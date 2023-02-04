namespace Oty.CommandLib.Utilities;

internal sealed class DiscordClientEventRegisterer : IDisposable
{
    private readonly DiscordClient _client;

    private readonly TypeInfo _clientTypeInfo;

    private readonly MethodInfo _genericEventMethod;

    private readonly object _genericEventInstance;

    private readonly object _locker = new();

    private readonly Dictionary<(Type HandlerType, CachedMethodType MethodType), Delegate> _expressionCache = new();

    private ImmutableDictionary<HandlerKey, Delegate> _registeredEvents = ImmutableDictionary<HandlerKey, Delegate>.Empty;

    internal DiscordClientEventRegisterer(DiscordClient client, MethodInfo genericEventMethod, object genericEventInstance)
    {
        this._client = client;
        this._clientTypeInfo = client.GetType().GetTypeInfo();

        this._genericEventMethod = genericEventMethod;

        this._genericEventInstance = genericEventInstance;
    }

    [PublicAPI]
    public void RegisterEvents(IEnumerable<HandlerKey> keys)
    {
        foreach (var type in keys)
        {
            this.RegisterEvent(type);
        }
    }

    [PublicAPI]
    public void RegisterEvent(HandlerKey key)
    {
        lock(this._locker)
        {
            var handlerType = typeof(AsyncEvent<,>).MakeGenericType(typeof(DiscordClient), key.EventType);

            var methodKey = (handlerType, CachedMethodType.AddEventHandler);

            if (!this._expressionCache.TryGetValue(methodKey, out var method))
            {
                var handler = this._clientTypeInfo.DeclaredFields.First(c => c.FieldType == handlerType)
                    .GetValue(this._client);

                method = handlerType.GetMethod("Register")!
                    .ToDelegate(handler);

                this._expressionCache.Add(methodKey, method);
            }

            var eventHandler = this.ToAsyncEventHandlerDelegate(key);

            method.DynamicInvoke(eventHandler);

            this._registeredEvents = this._registeredEvents.Add(key, eventHandler);
        }

    }

    [PublicAPI]
    public void TryRegisterEvent(HandlerKey key)
    {
        if (!this._registeredEvents.ContainsKey(key))
        {
            this.RegisterEvent(key);
        }
    }

    [PublicAPI]
    public void UnregisterAll()
    {
        foreach (var (type, _) in this._registeredEvents)
        {
            this.UnregisterEvent(type);
        }
    }

    public void Dispose()
    {
        this.UnregisterAll();
        this._expressionCache.Clear();
    }

    [PublicAPI]
    public void UnregisterEvent(HandlerKey key)
    {
        lock(this._locker)
        {
            var handlerType = typeof(AsyncEvent<,>).MakeGenericType(typeof(DiscordClient), key.EventType);

            var methodKey = (handlerType, CachedMethodType.RemoveEventHandler);

            if (!this._expressionCache.TryGetValue(methodKey, out var method))
            {
                var handler = this._clientTypeInfo.DeclaredFields.First(c => c.FieldType == handlerType)
                    .GetValue(this._client);

                method = handlerType.GetMethod("Register")!
                    .ToDelegate(handler);

                this._expressionCache.Add(methodKey, method);
            }

            var registeredEvent = this._registeredEvents[key];

            method.DynamicInvoke(registeredEvent);

            this._registeredEvents = this._registeredEvents.Remove(key);
        }
    }

    [PublicAPI]
    public void TryUnregisterEvent(HandlerKey key)
    {
        if (this._registeredEvents.ContainsKey(key))
        {
            this.UnregisterEvent(key);
        }
    }

    // resharper disable once CoVariantArrayConversion

    private Delegate ToAsyncEventHandlerDelegate(HandlerKey handlerKey)
    {
        var genericMethodInfo = this._genericEventMethod.MakeGenericMethod(handlerKey.EventType, handlerKey.CommandType, handlerKey.ContextType);

        var parameterTypes = genericMethodInfo.GetParameters().Select(t => t.ParameterType).ToArray();
        var expressionParameters = parameterTypes.Select(Expression.Parameter).ToArray();

        var instanceExpression = Expression.Constant(this._genericEventInstance);
        var body = Expression.Call(instanceExpression, genericMethodInfo, expressionParameters);

        var eventType = typeof(AsyncEventHandler<,>).MakeGenericType(parameterTypes);

        return Expression.Lambda(eventType, body, expressionParameters)
            .Compile();
    }
}