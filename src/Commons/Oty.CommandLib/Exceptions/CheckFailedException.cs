namespace Oty.CommandLib.Exceptions;

/// <summary>
/// Represents indication of check specified on <see cref="BaseCommandModule.BeforeExecutionAsync"/> failed.
/// </summary>
[Serializable]
public sealed class CheckFailedException : Exception
{
    /// <summary>
    /// Initializes an new instance of <see cref="CheckFailedException"/>.
    /// </summary>
    /// <param name="failedCommandMetadata">Metadata of the failed command.</param>
    /// <exception cref="ArgumentNullException"><oaramref name="failedCommandMetadata"/> is null.</exception>
    public CheckFailedException(BaseCommandMetadata failedCommandMetadata)
    {
        this.FailedCommandMetadata = failedCommandMetadata ?? throw new ArgumentNullException(nameof(failedCommandMetadata));
    }

    /// <summary>
    /// Initializes an new instance of <see cref="CheckFailedException"/> with an message.
    /// </summary>
    /// <param name="failedCommandMetadata">Metadata of the failed command.</param>
    /// <param name="message">Message describing the error.</param>
    /// <exception cref="ArgumentNullException"><oaramref name="failedCommandMetadata"/> is null.</exception>
    public CheckFailedException(BaseCommandMetadata failedCommandMetadata, string? message) : base(message)
    {
        this.FailedCommandMetadata = failedCommandMetadata; 
    }

    /// <summary>
    /// Initializes an new instance of <see cref="CheckFailedException"/> with an message.
    /// </summary>
    /// <param name="failedCommandMetadata">Metadata of the failed command.</param>
    /// <param name="message">Message describing the error.</param>
    /// <param name="innerException">The exception causing this.</param>
    /// <exception cref="ArgumentNullException"><paramref name="innerException"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><oaramref name="failedCommandMetadata"/> is null.</exception>
    public CheckFailedException(BaseCommandMetadata failedCommandMetadata, string? message, Exception? innerException) : base(message, innerException)
    {
        this.FailedCommandMetadata = failedCommandMetadata;
    }

    private CheckFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <summary>
    /// Gets the command with a failed check.
    /// </summary>
    public BaseCommandMetadata? FailedCommandMetadata { get; }
}
