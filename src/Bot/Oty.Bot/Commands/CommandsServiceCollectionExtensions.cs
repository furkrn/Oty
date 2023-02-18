namespace Oty.Bot.Commands;

public static class CommandsServiceCollectionExtensions
{
    [PublicAPI]
    public static IServiceCollection AddCommands(this IServiceCollection serviceCollection, Func<CommandsBuilder, CommandsBuilder> builderFunc)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
        ArgumentNullException.ThrowIfNull(builderFunc, nameof(builderFunc));

        return serviceCollection.AddCommands(builderFunc(new CommandsBuilder()));
    }

    [PublicAPI]
    public static IServiceCollection AddCommands(this IServiceCollection serviceCollection, CommandsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        serviceCollection.Configure<CommandsConfiguration>(config => config.RegisteredTypes = builder.Commands);

        serviceCollection.TryAddSingleton<ICommandsRegisterer, CommandsRegisterer>();
        serviceCollection.TryAddSingleton<ICheckCollection, CheckCollection>();
        serviceCollection.TryAddTransient(typeof(ICheckedRegistration<>), typeof(CheckedRegistration<>));

        return serviceCollection;
    }
}