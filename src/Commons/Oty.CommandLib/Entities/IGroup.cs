namespace Oty.CommandLib;

public interface IGroup<out TMetadata>
    where TMetadata : BaseCommandMetadata
{
    TMetadata Subcommand { get; }
}