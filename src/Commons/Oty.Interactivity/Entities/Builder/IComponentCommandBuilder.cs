using System.Collections.Generic;

namespace Oty.Interactivity.Entities;

public interface IComponentCommandBuilder
{
    IEnumerable<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>> Build();
}