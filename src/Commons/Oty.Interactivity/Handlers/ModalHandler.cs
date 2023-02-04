namespace Oty.Interactivity.Handlers;

internal sealed class ModalHandler : InteractivityHandler<ModalRequest, ModalSubmitEventArgs>,  IDisposable
{
    public ModalHandler(DiscordClient client) : base(client)
    {
        this.Client.ModalSubmitted += this.ModalSubmitted;
    }

    private Task ModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
    {
        var modalReqeust = this.Requests.Keys.FirstOrDefault(r => r.TargetInteractionId == e.Interaction.Data.CustomId);

        if (modalReqeust != null && modalReqeust.TargetUser == e.Interaction.User)
        {
            var modalInteractionData = this.Requests[modalReqeust];

            modalInteractionData.DiscordEvent = e;
            modalInteractionData.LastExecutedInteractionName = e.Interaction.Data.CustomId;
            modalInteractionData.ExecutionCount++;
            modalInteractionData.IsExecuted = true;
            modalInteractionData.Entries = e.Values;

            modalReqeust.TimedOutTaskCompletionSource.SetResult(false);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.Requests.Clear();
        this.Client.ModalSubmitted -= this.ModalSubmitted;
    }
}