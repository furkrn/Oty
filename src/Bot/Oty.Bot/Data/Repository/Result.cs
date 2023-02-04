namespace Oty.Bot.Data.Repository;

[PublicAPI]
public readonly struct Result<T> : IEquatable<Result<T>>
{
    [PublicAPI]
    public Result(bool isSuccessfull, T? value)
    {
        this.IsSuccessfull = isSuccessfull;
        this.Value = value;
    }

    [PublicAPI]
    [MemberNotNullWhen(true, nameof(IsSuccessfull))]
    public T? Value { get; }

    [PublicAPI]
    public bool IsSuccessfull { get; }

    [PublicAPI]
    public void Deconstruct(out T? value, out bool isSuccessfull)
    {
        value = this.Value;
        isSuccessfull = this.IsSuccessfull;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Result<T> other && this.Equals(other);
    }

    public bool Equals(Result<T> other)
    {
        return EqualityComparer<T>.Default.Equals(this.Value, other.Value) &&
            this.IsSuccessfull == other.IsSuccessfull;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.IsSuccessfull);
    }

    public static implicit operator T?(Result<T> result)
        => result.Value;

    public static bool operator ==(Result<T> left, Result<T> right)
        => left.Equals(right);

    public static bool operator !=(Result<T> left, Result<T> right)
        => !(left == right);
}
