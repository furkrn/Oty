namespace Oty.Bot.Infastructure;

public class AsyncEventHandlerDelegateCreator : IAsyncEventHandlerDelegateCreator
{
    public Delegate CreateDelegate(IAsyncEventHandler instance, Type senderType, Type eventArgsType)
    {
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));
        ArgumentNullException.ThrowIfNull(senderType, nameof(senderType));
        ArgumentNullException.ThrowIfNull(eventArgsType, nameof(eventArgsType));

        if (!typeof(AsyncEventArgs).IsAssignableFrom(eventArgsType))
        {
            throw new ArgumentException("Invalid type specified", nameof(eventArgsType));
        }

        var genericHandlerType = typeof(IAsyncEventHandler<,>).MakeGenericType(senderType, eventArgsType);

        if (!genericHandlerType.IsAssignableFrom(instance.GetType()))
        {
            throw new ArgumentException("Instance should match with its sender and event args type.", nameof(instance));
        }

        var senderParameter = Expression.Parameter(senderType, "sender");
        var eventArgsParameter = Expression.Parameter(eventArgsType, "e");

        var instanceExpression = Expression.Constant(instance, genericHandlerType);
        var callExpression = Expression.Call(instanceExpression, genericHandlerType.GetMethod("ExecuteAsync")!, senderParameter, eventArgsParameter);

        var expressionBlock = Expression.Block(instanceExpression, instanceExpression, callExpression);

        var genericHandlerDelegateType = typeof(AsyncEventHandler<,>).MakeGenericType(senderType, eventArgsType);

        return Expression.Lambda(genericHandlerDelegateType, expressionBlock, senderParameter, eventArgsParameter)
            .Compile();
    }
}