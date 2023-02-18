namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public interface ICheckedRegistration
{
    [PublicAPI]
    bool CanExecute { get; }
}

[PublicAPI]
public interface ICheckedRegistration<[UsedImplicitly] out TModule> : ICheckedRegistration
    where TModule : BaseCommandModule
{
}