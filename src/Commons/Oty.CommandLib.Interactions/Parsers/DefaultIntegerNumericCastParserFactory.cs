namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// Factory of integer and floating-point number caster.
/// </summary>
public sealed class DefaultIntegerNumericCastParserFactory : IParameterTypeParserFactory
{
    private static readonly Type[] SupportedTypes = new[]
    {
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(ulong),
        typeof(nint),
        typeof(nuint),
        typeof(float),
        typeof(decimal),
    };

    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Values[0] is (double or long) && SupportedTypes.Contains(parserContext.Option.Type);
    }

    /// <inheritdoc/>
    public IParameterTypeParser? CreateParser(ITypeParserContext parserContext)
    {
        var rawValueType = parserContext.Values[0]!.GetType();

        var parserType = typeof(DefaultValueCastingParser<,>).MakeGenericType(rawValueType, parserContext.Option.Type!);

        return (IParameterTypeParser?)Activator.CreateInstance(parserType);
    }
}