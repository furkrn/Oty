namespace Oty.Bot.Infastructure;

public static class BuilderExtensionsMethods
{
    [PublicAPI]
    public static InteractionCommandBuilder<TCommand> LocalizeFrom<TCommand>(this InteractionCommandBuilder<TCommand> builder, IMetadataProvider provider, 
        Func<ITranslationCollection<TCommand>, IReadOnlyDictionary<string, string?>?>? nameFunc = null,
        Func<ITranslationCollection<TCommand>, IReadOnlyDictionary<string, string?>?>? descriptionFunc = null)
        where TCommand : BaseCommandModule
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        nameFunc ??= ln => ln[builder.CommandName];
        descriptionFunc ??= builder.Description is not null ? ld => ld[builder.Description] : null;

        AddLocalizedValues(builder, provider,
            (b, culture, values) => b.AddLocalizedValues(culture, values),
            nameFunc,
            descriptionFunc);

        return builder;
    }

    [PublicAPI]
    public static SlashInteractionCommandMetadataOptionBuilder LocalizeFrom<TResourceType>(this SlashInteractionCommandMetadataOptionBuilder builder, IMetadataProvider provider,
        Func<ITranslationCollection<TResourceType>, IReadOnlyDictionary<string, string?>?>? nameFunc = null,
        Func<ITranslationCollection<TResourceType>, IReadOnlyDictionary<string, string?>?>? descriptionFunc = null)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        nameFunc ??= ln => ln[builder.Name];
        descriptionFunc ??= builder.Description is not null ? ld => ld[builder.Description] : null;

        AddLocalizedValues(builder, provider,
            (b, culture, values) => b.AddLocalizedValues(culture, values),
            nameFunc,
            descriptionFunc);

        return builder;
    }

    private static void AddLocalizedValues<TBuilder, TResourceType>(TBuilder builder, 
        IMetadataProvider provider,
        Func<TBuilder, string, LocalizedValues, TBuilder> localizationFunc,
        Func<ITranslationCollection<TResourceType>, IReadOnlyDictionary<string, string?>?> nameFunc,
        Func<ITranslationCollection<TResourceType>, IReadOnlyDictionary<string, string?>?>? descriptionFunc)
    {
        var collection = provider.Services!.GetRequiredService<ITranslationCollection<TResourceType>>();

        var nameDictionary = nameFunc(collection);
        var descriptionDictionary = descriptionFunc?.Invoke(collection);

        var localizations = nameDictionary?.Select(kp => new { kp.Key, Values = LocalizedValues.Create(kp.Value,
            descriptionDictionary?.TryGetValue(kp.Key, out var description) == true ? description : null)})
            .ToDictionary(kp => kp.Key, kp => kp.Values);

        if (localizations is not null)
        {
            foreach (var (culture, values) in localizations)
            {
                localizationFunc(builder, culture, values);
            }
        }
    }
}