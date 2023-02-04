namespace Oty.CommandLib.Entities;

public interface IGroupableMetadata<out TMetadata>
    where TMetadata : BaseCommandMetadata
{
    IReadOnlyList<IGroup<TMetadata>>? Groups { get; }
}