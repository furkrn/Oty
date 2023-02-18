namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public class CheckedRegistration<TModule> : ICheckedRegistration<TModule>
    where TModule : BaseCommandModule
{
    private readonly ICheckedRegistration _registration;

    [PublicAPI]
    public CheckedRegistration(ICheckCollection collection)
    {
        this._registration = collection[typeof(TModule)];
    }

    [PublicAPI]
    public bool CanExecute => this._registration.CanExecute;
}