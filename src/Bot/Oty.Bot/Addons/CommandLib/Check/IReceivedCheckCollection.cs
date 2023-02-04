namespace Oty.Bot.Addons.CommandLib.Checks;

public interface IReceivedCheckCollection : IEnumerable<IReceivedCheck>
{
    void Add<T>(IReceivedCheck<T> check)
        where T : IReceivedValue;

    Task<bool> CheckAsync(AddonContext context);
}