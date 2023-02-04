namespace Oty.CommandLib.Interactions.Parsers;

internal sealed class DefaultValueCastingParser<TRaw, T> : IParameterTypeParser<T>
    where TRaw : notnull
{
    private readonly Func<TRaw, T?> _converter;

    public DefaultValueCastingParser()
    {
        var rawParameterExpression = Expression.Parameter(typeof(TRaw));
        var converterExpression = Expression.Convert(rawParameterExpression, typeof(T));

        this._converter = Expression.Lambda<Func<TRaw, T?>>(converterExpression, rawParameterExpression).Compile();
    }

    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Option.Type == typeof(T);
    }

    public Task<Optional<T?>> ConvertValueAsync(ITypeParserContext parserContext)
    {
        if (parserContext.Values is not TRaw rawValue)
        {
            return Task.FromResult(Optional.FromNoValue<T?>());
        }

        var expressionValue = this._converter(rawValue);

        return Task.FromResult(Optional.FromValue(expressionValue));
    }
}