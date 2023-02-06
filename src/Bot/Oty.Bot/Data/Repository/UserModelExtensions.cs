namespace Oty.Bot.Data.Repository;

public static class UserModelExtensions
{
    public static async Task<Result<User>> TryLiftUserBan(this IUserRepository repository, ulong userId)
    {
        var user = await repository.GetUserAsync(userId);

        if (user is null ||
            user.UserState is UserStates.TosAccepted)
        {
            return new(true, user);
        }

        var now = DateTime.Now;
        if (user.BanLiftTime.GetValueOrDefault() > now)
        {
            return new(false, user);
        }

        await repository.UpdateUserAsync(userId, user => 
        {
            user.UserState = UserStates.TosAccepted;
            user.BanLiftTime = null;
            user.BanReason = null;
        });

        await repository.UnitOfWork.SaveChangesAsync();

        return new(true, user);
    }
}