namespace Oty.CommandLib;

/// <summary>
/// Defines which method is the target method to execute on a command module.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[PublicAPI]
public sealed class TargetCommandMethodAttribute : Attribute
{
}