namespace Oty.CommandLib.Interactions.Utilities;

internal static class InteractionCommandBuilderUtilityExtensions
{
    public static IEnumerable<DiscordInteractionDataOption>? GetInteractionOptions(this DiscordInteraction interaction)
    {
        if (interaction.Data.Options?.Any(o => o.Type is (ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup)) != true)
        {
            return interaction.Data.Options;
        }
            
        var option = interaction.Data.Options.First();

        return EnumerableExtensions.Traverse(option, o => o.Options)
            .First(o => o.Type == ApplicationCommandOptionType.SubCommand)
            .Options;
    }

    public static bool IsValidSlashCommandName(this string name)
    {
        var regex = new Regex(@"^[\w-]{1,32}$");
        return regex.IsMatch(name) && !name.Any(char.IsUpper);
    }

    public static DiscordOptionRange? SetDefaultRange(Type? type)
    {
        ValueTuple<long?, long?>? tuple = type switch
        {
            _ when type == typeof(byte) => new ValueTuple<long, long>(byte.MinValue, byte.MaxValue),
            _ when type == typeof(sbyte) => new ValueTuple<long, long>(sbyte.MinValue, sbyte.MaxValue),
            _ when type == typeof(int) => new ValueTuple<long, long>(int.MinValue, int.MaxValue),
            _ when type == typeof(uint) => new ValueTuple<long, long>(uint.MinValue, uint.MaxValue),
            _ when type == typeof(short) => new ValueTuple<long, long>(short.MinValue, short.MaxValue),
            _ when type == typeof(ushort) => new ValueTuple<long, long>(ushort.MinValue, ushort.MaxValue),
            _ when type == typeof(ulong) => new ValueTuple<long, long?>(0, null),
            _ => null,
        };

        return tuple.HasValue ? new DiscordOptionRange(tuple.Value.Item1, tuple.Value.Item2) : null;
    }

    // resharper disable all PossibleNullReferenceException 

    public static bool IsRangeCorrect(Type? type, DiscordOptionRange range)
    {
        (long? minimum, long? maximum) = type switch
        {
            _ when type == typeof(byte) => new ValueTuple<long, long>(byte.MinValue, byte.MaxValue),
            _ when type == typeof(sbyte) => new ValueTuple<long, long>(sbyte.MinValue, sbyte.MaxValue),
            _ when type == typeof(int) => new ValueTuple<long, long>(int.MinValue, int.MaxValue),
            _ when type == typeof(uint) => new ValueTuple<long, long>(uint.MinValue, uint.MaxValue),
            _ when type == typeof(short) => new ValueTuple<long, long>(short.MinValue, ushort.MaxValue),
            _ when type == typeof(ushort) => new ValueTuple<long, long>(ushort.MinValue, ushort.MaxValue),
            _ when type == typeof(ulong) => new ValueTuple<long, long?>(0, null),
            _ => (0, 0),
        };

        if (range.MinimumValue is double)
        {
            return true;
        }

        return minimum <= (long?)range.MinimumValue && (long?)range.MaximumValue <= maximum;
    }

    // resharper restore all

    public static bool HasCorrectParameterOrdering(MethodInfo methodInfo, IList<Type?> types)
    {
        var parameterTypes = methodInfo.GetParameters().Select(p => Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType);

        var intersectedResult = parameterTypes.Where(types.Contains).ToArray();

        return intersectedResult.Length == types.Count;
    }

    public static bool IsOptionRequirementOrderingNotCorrect(SlashInteractionCommandOption option, SlashInteractionCommandOption? requiredOption)
    {
        return requiredOption?.DefaultValue.HasValue == true && !option.DefaultValue.HasValue;
    }
}