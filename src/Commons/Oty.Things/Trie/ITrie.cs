namespace Oty.Things.Trie;

public interface ITrie<T>
{
    IEnumerable<T> this[string query] { get; }

    IReadOnlyList<T> AvaliableValues { get; }

    void Add(string query, T value);

    IEnumerable<T> Get(string query);
}