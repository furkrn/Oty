namespace Oty.Bot.Addons.CommandLib.Checks;

public interface IReceivedCheck
{
    IReceivedValue? NewReceived { get; }

    Task<bool> CanExecute(IReceivedValue value, AddonContext context);
}

public interface IReceivedCheck<in TReceived> : IReceivedCheck
    where TReceived : IReceivedValue
{
    Task<bool> IReceivedCheck.CanExecute(IReceivedValue value, AddonContext context)
    {
        if (value is TReceived received)
        {
            return this.CanExecute(received, context);
        }

        return Task.FromResult(false);
    }

    Task<bool> CanExecute(TReceived value, AddonContext context);
}