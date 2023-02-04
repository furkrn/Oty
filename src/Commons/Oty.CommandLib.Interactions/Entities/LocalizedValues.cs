using System.Diagnostics.CodeAnalysis;

namespace Oty.CommandLib.Interactions.Entities;

public readonly struct LocalizedValues : IEquatable<LocalizedValues>
{
    private LocalizedValues(string? name, string? description)
    {
        this.Name = name;
        this.Description = description;
    }

    public string? Name { get; }

    public string? Description { get; }

    public void Deconstruct(out string? name, out string? description)
    {
        name = this.Name;
        description = this.Description;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is LocalizedValues other && this.Equals(other);
    }

    public bool Equals(LocalizedValues other)
    {
        return this.Name == other.Name &&
            this.Description == other.Description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name, this.Description);
    }

    public static LocalizedValues Create(string? name, string? description)
    {
        return new(name, description);
    }

    public static bool operator ==(LocalizedValues left, LocalizedValues right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LocalizedValues left, LocalizedValues right)
    {
        return !(left == right);
    }
}