namespace Oty.Interactivity.Entities;

/// <summary>
/// Builder class for building click-based component interactions.
/// </summary>
public sealed class ClickInteractivityRequestBuilder
{
    /// <summary>
    /// Gets the whether button can be pressed for multiple times.
    /// </summary>
    public bool IsRepeative { get; private set; }

    /// <summary>
    /// Gets the target interaction's custom id.
    /// </summary>
    public string TargetInteractionId { get; private set; }

    /// <summary>
    /// Gets the target user of the interaction.
    /// </summary>
    public DiscordUser TargetUser { get; private set; }

    /// <summary>
    /// Gets the target message of ther interaction.
    /// </summary>
    public DiscordMessage TargetMessage { get; private set; }

    /// <summary>
    /// Gets the task that will execute when it's "repeative".
    /// </summary>
    public Func<ComponentInteractionCreateEventArgs, Task> RepeativeExecutionTask { get; private set; } = (_) => Task.CompletedTask;

    /// <summary>
    /// TODO : Documnet this later
    /// </summary>
    public ClickInteractivityRequestBuilder WithTargetInteractionId(string interactionId)
    {
        this.TargetInteractionId = interactionId ?? throw new ArgumentNullException(nameof(interactionId));

        return this;
    }

    /// <summary>
    /// Sets the target user of the request.
    /// </summary>
    /// <param name="user"></param>
    public ClickInteractivityRequestBuilder SetTargetUser(DiscordUser user)
    {
        this.TargetUser = user ?? throw new ArgumentNullException(nameof(user));

        return this;
    }

    /// <summary>
    /// Sets whether request only works with single clickç
    /// </summary>
    /// <param name="isRepeative"></param>
    public ClickInteractivityRequestBuilder SetRepeative(bool isRepeative)
    {
        this.IsRepeative = isRepeative;

        return this;
    }

    /// <summary>
    /// Sets the target message of the request.
    /// </summary>
    /// <param name="message"></param>
    public ClickInteractivityRequestBuilder WithTargetMessage(DiscordMessage message)
    {
        this.TargetMessage = message ?? throw new ArgumentNullException(nameof(message));

        return this;
    }

    /// <summary>
    /// Sets the task that will execute multiple times, this is effective when <see cref="this.IsRepeative"></see> is <see langword="true"></see>
    /// </summary>
    /// <param name="executionTask"></param>
    public ClickInteractivityRequestBuilder WithRepeativeTask(Func<ComponentInteractionCreateEventArgs, Task> executionTask)
    {
        this.RepeativeExecutionTask = executionTask ?? throw new ArgumentNullException(nameof(executionTask));

        return this;
    }

    /// <summary>
    /// Builds a request from this builder.
    /// </summary>
    /// <returns></returns>
    [PublicAPI]
    public ClickInteractivityRequest Build()
    {
        return new ClickInteractivityRequest(this);
    }

    /// <summary>
    /// Clears specified values on this builder.
    /// </summary>
    public void Clear()
    {
        this.IsRepeative = false;
        this.TargetInteractionId = null;
        this.TargetUser = null;
        this.TargetMessage = null;
        this.RepeativeExecutionTask = (_) => Task.CompletedTask;
    }

    public static implicit operator ClickInteractivityRequest(ClickInteractivityRequestBuilder builder)
        => builder!.Build();
}