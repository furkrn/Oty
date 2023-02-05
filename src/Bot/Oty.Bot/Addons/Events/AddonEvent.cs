namespace Oty.Bot.Addons.Events;

public class AddonEvent<TSender, TArgs> : IAsyncEventHandler<TSender, TArgs>
    where TSender : TArgs
{
    public Task ExecuteAsync(TSender sender, TArgs e)
    {
        throw new NotImplementedException();    
    }
}