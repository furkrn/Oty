namespace Oty.CommandLib.Interactions;

public sealed class InteractionCommandConfiguration
{
    public bool PublishPublishables { internal get; set; } = true;

    public bool PublishWhenClientReady { internal get; set; } = true;
}