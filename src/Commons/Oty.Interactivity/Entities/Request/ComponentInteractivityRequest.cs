using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Oty.Interactivity.Entities;

[PublicAPI]
public sealed class ComponentInteractivityRequest : InteractivityRequest, IEquatable<ComponentInteractivityRequest>
{
    internal ComponentInteractivityRequest()
    {
    }

    [PublicAPI]
    public IReadOnlyDictionary<ComponentKey, ComponentInteractivityInvoker?> TargetComponents { get; internal init; }

    [PublicAPI]
    public DiscordUser? TargetUser { get; internal init; }

    [PublicAPI]
    public DiscordMessage TargetMessage { get; internal init; }

    [PublicAPI]
    public bool IsRepeative { get; internal init; }

    public override bool Equals(object? obj)
    {
        return obj is ComponentInteractivityRequest request && this.Equals(request);
    }

    public bool Equals(ComponentInteractivityRequest? other)
    {
        return other is not null &&
            EqualityComparer<IReadOnlyDictionary<ComponentKey, ComponentInteractivityInvoker?>>.Default.Equals(this.TargetComponents, other.TargetComponents) &&
            this.TargetUser == other.TargetUser &&
            this.TargetMessage == other.TargetMessage &&
            this.IsRepeative == other.IsRepeative &&
            this.TimedOutTaskCompletionSource == other.TimedOutTaskCompletionSource;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.TargetComponents, this.TargetUser, this.TargetMessage);
    }

    public static bool operator ==(ComponentInteractivityRequest left, ComponentInteractivityRequest right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ComponentInteractivityRequest left, ComponentInteractivityRequest right)
    {
        return !(left == right);
    }

    [PublicAPI]
    public sealed class ComponentInteractivityInvoker
    {
        public ComponentInteractivityInvoker(string targetComponentId, ComponentInvokationType type, Func<ComponentInteractionCreateEventArgs, Task>? caller)
            : this(targetComponentId, caller)
        {
            this.ComponentInvokationType = type;
        }

        public ComponentInteractivityInvoker(string targetComponentId, ComponentType componentType, Func<ComponentInteractionCreateEventArgs, Task>? caller)
            : this(targetComponentId, caller)
        {
            if (componentType is (ComponentType.ActionRow or ComponentType.FormInput))
            {
                throw new ArgumentException("Specified Component Type can be only Button or Select", nameof(componentType));
            }

            this.ComponentInvokationType = (ComponentInvokationType)componentType;
        }

        private ComponentInteractivityInvoker(string targetComponentId, Func<ComponentInteractionCreateEventArgs, Task>? caller)
        {
            if (string.IsNullOrWhiteSpace(targetComponentId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetComponentId));
            }

            this.TargetComponentId = targetComponentId;
            this.Caller = caller;
        }

        [PublicAPI]
        public string TargetComponentId { get; }

        [PublicAPI]
        public ComponentInvokationType ComponentInvokationType { get; }

        [PublicAPI]
        public Func<ComponentInteractionCreateEventArgs, Task>? Caller { get; }
    }

    [PublicAPI]
    public readonly struct ComponentKey : IEquatable<ComponentKey>
    {
        private readonly string _targetComponentId;

        public ComponentKey(string targetComponentId, ComponentInvokationType type)
        {
            if (string.IsNullOrWhiteSpace(targetComponentId))
            {
                throw new ArgumentException("Value cannot be null or whitespace", nameof(this.TargetComponentId));
            }

            this._targetComponentId = targetComponentId;
            this.ComponentInvokationType = type;
        }

        [PublicAPI]
        public string TargetComponentId
        {
            get
            {
                return this._targetComponentId;
            }
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value cannot be null or whitespace", nameof(this.TargetComponentId));
                }

                this._targetComponentId = value;
            }
        }

        [PublicAPI]
        public ComponentInvokationType ComponentInvokationType { get; init; }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is ComponentKey key && this.Equals(key);
        }

        public bool Equals(ComponentKey other)
        {
            return this.TargetComponentId == other.TargetComponentId &&
                this.ComponentInvokationType == other.ComponentInvokationType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.TargetComponentId, this.ComponentInvokationType);
        }

        public static bool operator ==(ComponentKey right, ComponentKey left)
        {
            return right.Equals(left);
        }

        public static bool operator !=(ComponentKey right, ComponentKey left)
        {
            return !(right == left);
        }

        [PublicAPI]
        public static implicit operator ComponentKey(DiscordInteractionData data)
        {
            return new()
            {
                TargetComponentId = data.CustomId,
                ComponentInvokationType = (ComponentInvokationType)data.ComponentType,
            };
        }

        [PublicAPI]
        public static explicit operator ComponentKey(DiscordComponent component)
        {
            if (component.Type is (ComponentType.ActionRow or ComponentType.FormInput))
            {
                throw new ArgumentException("Specified componenty", nameof(component));
            } 

            return new()
            {
                TargetComponentId = component.CustomId,
                ComponentInvokationType = (ComponentInvokationType)component.Type
            };
        }
    }
}