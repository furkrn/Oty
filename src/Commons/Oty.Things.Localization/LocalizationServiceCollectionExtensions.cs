namespace Oty.Things.Localization;

public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers PO based localizations to service
    /// </summary>
    /// <param name="serviceCollection">Service collection.</param>
    /// <param name="optionAction">Configures the localization options.</param>
    /// <returns></returns>
    [PublicAPI]
    public static IServiceCollection AddPoLocalization(this IServiceCollection serviceCollection, Action<PoLocalizationOptions> optionAction)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
        ArgumentNullException.ThrowIfNull(optionAction, nameof(optionAction));

        serviceCollection.TryAddSingleton<IPoProvider, PoProvider>();
        serviceCollection.TryAddSingleton<IStringLocalizerFactory, PoStringLocalizerFactory>();
        serviceCollection.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        serviceCollection.TryAddSingleton<ITranslationCollectionFactory, TranslationCollectionFactory>();
        serviceCollection.TryAddTransient(typeof(ITranslationCollection<>), typeof(TranslationCollection<>));

        serviceCollection.Configure(optionAction);

        return serviceCollection;
    }
}