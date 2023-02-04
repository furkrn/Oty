namespace Oty.Bot.Infastructure;

public interface IAsyncEventRegistrationCreator
{
    RegistrationHandler CreateRegisterationDelegate<TSender, TArgs, THandler>()
        where TArgs : AsyncEventArgs
        where THandler : IAsyncEventHandler<TSender, TArgs>;

    RegistrationHandler CreateRegisterationDelegate(Type senderType, Type eventArgsType, Type eventHandler);

    RegistrationHandler CreateUnregisterationDelegate<TSender, TArgs, THandler>()
        where TArgs : AsyncEventArgs
        where THandler : IAsyncEventHandler<TSender, TArgs>;

    RegistrationHandler CreateUnregisterationDelegate(Type senderType, Type eventArgsType, Type eventHandler);
}