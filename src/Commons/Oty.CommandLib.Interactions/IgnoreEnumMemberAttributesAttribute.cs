namespace Oty.CommandLib.Interactions.Attributes;

/// <summary>
/// Indicates <see cref="DefaultEnumChoiceProvider"/> will ignore <see cref="EnumMemberAttribute"/> for providing choices via enums.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
[PublicAPI]
public sealed class IgnoreEnumMemberAttributesAttribute : Attribute
{
}