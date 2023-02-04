namespace Oty.Bot.Infastructure;

public interface IAsyncEventHandlerDelegateCreator
{
    Delegate CreateDelegate(IAsyncEventHandler instance, Type senderType, Type eventArgsType);
}