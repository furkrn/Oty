using DSharpPlus.Entities;
using JetBrains.Annotations;
using System;

namespace Oty.Interactivity.Entities;

[PublicAPI]
public sealed class ModalRequest : InteractivityRequest
{
    public ModalRequest(DiscordUser targetUser, string targetInteractionId)
    {
        this.TargetUser = targetUser ?? throw new ArgumentNullException(nameof(targetUser));

        if (string.IsNullOrWhiteSpace(targetInteractionId))
        {
            throw new ArgumentException("Specified value cannot be null or whitespace", nameof(targetInteractionId));
        }

        this.TargetInteractionId = targetInteractionId;
    }

    [PublicAPI]
    public DiscordUser TargetUser { get; }

    [PublicAPI]
    public string TargetInteractionId { get; }
}