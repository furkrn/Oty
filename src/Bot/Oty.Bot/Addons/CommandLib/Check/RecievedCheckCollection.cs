namespace Oty.Bot.Addons.CommandLib.Checks;

public sealed class ReceivedCheckCollection : IReceivedCheckCollection
{
    private readonly ConcurrentDictionary<Type, IReceivedCheck> _receivedCheck = new();

    public void Add<T>(IReceivedCheck<T> value)
        where T : IReceivedValue
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        this._receivedCheck.TryAdd(typeof(T), value);
    }

    public Task<bool> CheckAsync(AddonContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var type = context.Received.GetType();
        var check = this._receivedCheck[type];

        if (check is null)
        {
            throw new InvalidOperationException($"No check registered for type {type.Name}");
        }

        return check.CanExecute(context.Received, context);
    }

    public IEnumerator<IReceivedCheck> GetEnumerator()
    {
        return this._receivedCheck.Select(c => c.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}