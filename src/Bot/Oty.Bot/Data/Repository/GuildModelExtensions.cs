namespace Oty.Bot.Data.Repository;

public static class GuildModelExtensions
{
    public static async Task<Result<Guild>> TryLiftBan(this IGuildRepository repository, ulong guildId)
    {
        var (guild, hasGuild) = await repository.HasGuildAsync(guildId);

        if (!hasGuild || guild is null
            || guild.GuildState is GuildStates.Verified)
        {
            return new(true, guild);
        }

        var now = DateTime.Now;
        if (guild.LiftTime.GetValueOrDefault() > now)
        {
            return new(false, guild);
        }

        await repository.UpdateGuildAsync(guild, UpdateGuild);
        await repository.UnitOfWork.SaveChangesAsync();

        return new(true, guild);

        void UpdateGuild(Guild guild)
        {
            guild.LiftTime = null;
            guild.GuildState = GuildStates.Verified;
            guild.RestrictionReason = null;
        }
    }
}