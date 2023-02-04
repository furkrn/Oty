namespace Oty.Bot.Infastructure;

public sealed class GuildRequirementsBuilder
{
    private readonly Dictionary<string, bool> _requirements = new();

    public GuildRequirementsBuilder(DiscordGuild guild)
    {
        this.Guild = guild;
    }

    public DiscordGuild Guild { get; }

    public IReadOnlyDictionary<string, bool> Requirements
        => this._requirements;

    public GuildRequirementsBuilder AddRequirement(Func<DiscordGuild, bool> requirementCheck, string failMessage)
    {
        ArgumentNullException.ThrowIfNull(requirementCheck, nameof(requirementCheck));
        ArgumentNullException.ThrowIfNullOrEmpty(failMessage, nameof(failMessage));

        this._requirements.Add(failMessage, requirementCheck(this.Guild));

        return this;
    }

    public GuildRequirements Build()
    {
        return new GuildRequirements(this);
    }
}