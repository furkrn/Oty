namespace Oty.Things.AsyncMonitor.AsyncEvents;

/// <summary>
/// Encapsulates a event task method for the specified event.
/// </summary>
/// <param name="sender">The sender of the event.</param>
/// <param name="eventArgs">The argument of the event.</param>
/// <typeparam name="TSender">The sender of the event.</typeparam>
/// <typeparam name="TEventArgs">The argument of the event.</typeparam>
/// <returns></returns>
public delegate Task AsyncEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs eventArgs)
    where TEventArgs : IAsyncEventArgs;