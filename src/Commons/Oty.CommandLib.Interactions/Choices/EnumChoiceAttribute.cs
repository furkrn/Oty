namespace Oty.CommandLib.Interactions;

/// <summary>
/// Provides enum application command option choices instead of its name or using <see cref="EnumMemberAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
[PublicAPI]
public sealed class EnumChoiceAttribute : Attribute
{
    /// <summary>
    /// Defines a attribute to a an enum.
    /// </summary>
    /// <param name="name">Specified name for the specified enum value.</param>
    public EnumChoiceAttribute(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets the Name of the application command option for specified enum member.
    /// </summary>
    [PublicAPI]
    public string Name { get; }
}