namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public class CheckCollection : ICheckCollection
{
    private readonly ConcurrentDictionary<Type, List<bool>> _registrationResults = new();

    private static readonly ICheckedRegistration _defaultValue = new CheckedRegistration(false);

    [PublicAPI]
    public ICheckedRegistration this[Type type]
        => this.GetRegistrationCheckOrDefault(type);

    [PublicAPI]
    public void AddResult(Type type, bool isFailed)
    {
        ValidateType(type);

        var list = this._registrationResults.GetOrAdd(type, _ => new());
        list.Add(isFailed);
    }

    [PublicAPI]
    public ICheckedRegistration GetRegistrationCheckOrDefault(Type type)
    {
        ValidateType(type);

        this._registrationResults.TryGetValue(type, out var list);

        if (list is null)
        {
            return _defaultValue;
        }

        bool canExecute = list.TrueForAll(c => c);
        return new CheckedRegistration(canExecute);
    }

    [PublicAPI]
    public bool HasResults(Type type)
    {
        ValidateType(type);

        return this._registrationResults.TryGetValue(type, out _);
    }

    private static void ValidateType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        if (!typeof(BaseCommandModule).IsAssignableFrom(type))
        {
            throw new ArgumentException("", nameof(type));
        }
    }
}