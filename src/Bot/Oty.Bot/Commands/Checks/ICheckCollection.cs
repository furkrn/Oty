namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public interface ICheckCollection
{
    [PublicAPI]
    ICheckedRegistration this[Type type] { get; }

    [PublicAPI]
    void AddResult(Type type, bool isFailed);

    [PublicAPI]
    ICheckedRegistration GetRegistrationCheckOrDefault(Type type);

    [PublicAPI]
    bool HasResults(Type type);
}