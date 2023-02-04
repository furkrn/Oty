namespace Oty.Bot.Infastructure;

public class AsyncEventRegistrationCreator : IAsyncEventRegistrationCreator
{
    private readonly ConcurrentDictionary<(DelegateType Type, Type Sender, Type EventArgs, string? EventName), RegistrationHandler> _cache = new();

    public RegistrationHandler CreateRegisterationDelegate<TSender, TArgs, THandler>()
        where TArgs : AsyncEventArgs
        where THandler : IAsyncEventHandler<TSender, TArgs>
    {
        return this.CreateRegisterationDelegate(typeof(TSender), typeof(TArgs), typeof(THandler));
    }

    public RegistrationHandler CreateRegisterationDelegate(Type senderType, Type eventArgsType, Type handlerType)
    {
        Validate(senderType, eventArgsType, handlerType, out string? eventName);

        return this._cache.GetOrAdd((DelegateType.Registration, senderType, eventArgsType, eventName),
            CreateDelegate(senderType, eventArgsType, eventName, e => e.GetAddMethod()!));
    }

    public RegistrationHandler CreateUnregisterationDelegate<TSender, TArgs, THandler>()
        where TArgs : AsyncEventArgs
        where THandler : IAsyncEventHandler<TSender, TArgs>
    {
        return this.CreateUnregisterationDelegate(typeof(TSender), typeof(TArgs), typeof(THandler));
    }

    public RegistrationHandler CreateUnregisterationDelegate(Type senderType, Type eventArgsType, Type handlerType)
    {
        Validate(senderType, eventArgsType, handlerType, out string? eventName);

        return this._cache.GetOrAdd((DelegateType.Unregistration, senderType, eventArgsType, eventName),
            CreateDelegate(senderType, eventArgsType, eventName, e => e.GetRemoveMethod()!));
    }

    private static void Validate(Type? senderType, Type? eventArgsType, Type handlerType, out string? eventName)
    {
        ArgumentNullException.ThrowIfNull(senderType, nameof(senderType));
        ArgumentNullException.ThrowIfNull(eventArgsType, nameof(eventArgsType));
        ArgumentNullException.ThrowIfNull(handlerType, nameof(handlerType));

        if (!typeof(AsyncEventArgs).IsAssignableFrom(eventArgsType))
        {
            throw new ArgumentException("Wrong type is specified.", nameof(eventArgsType));
        }

        var genericHandlerType = typeof(IAsyncEventHandler<,>).MakeGenericType(senderType, eventArgsType);

        if (!genericHandlerType.IsAssignableFrom(handlerType))
        {
            throw new ArgumentException("Wrong handler type is specified.", nameof(handlerType));
        }

        eventName = handlerType.GetCustomAttribute<EventNameAttribute>()?.Event;
    }

    private static RegistrationHandler CreateDelegate(Type senderType, Type eventArgsType, string? eventName, Func<EventInfo, MethodInfo> eventMethodFunc)
    {
        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var asyncEventArgsParameter = Expression.Parameter(typeof(Delegate), "eventHandler");

        var asyncEventHandlerType = typeof(AsyncEventHandler<,>).MakeGenericType(senderType, eventArgsType);

        var instanceCastExpression = Expression.Convert(instanceParameter, senderType);
        var eventArgsCastExpression = Expression.Convert(asyncEventArgsParameter, asyncEventHandlerType);

        var targetEventsArray = senderType.GetEvents()
            .Where(e => e.EventHandlerType == asyncEventHandlerType && (eventName is null || e.Name == eventName))
            .ToArray();

        if (targetEventsArray.Length is not 1)
        {
            throw new InvalidOperationException();
        }

        var targetMethod = eventMethodFunc(targetEventsArray[0]);

        var callExpression = Expression.Call(instanceCastExpression, targetMethod, eventArgsCastExpression);

        var block = Expression.Block(instanceParameter, asyncEventArgsParameter, instanceCastExpression, eventArgsCastExpression, callExpression);

        return Expression.Lambda<RegistrationHandler>(block, instanceParameter, asyncEventArgsParameter)
            .Compile();
    }

    private enum DelegateType
    {
        Registration,

        Unregistration
    }
}