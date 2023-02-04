namespace Oty.Things.Trie;

public interface IReadOnlyTrie<out T>
{
    IEnumerable<T> this[string query] { get; }

    IEnumerable<T> Get(string query);
}