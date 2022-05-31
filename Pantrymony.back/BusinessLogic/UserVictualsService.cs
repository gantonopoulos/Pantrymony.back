using Pantrymony.back.Model;
using static Pantrymony.back.DataAccess.UserVictualsAccess;

namespace Pantrymony.back.BusinessLogic;

public static class UserVictualsService
{
    public static async Task<List<Victual>> FetchUserVictualsAsync(string userId, string victualId, ILogger logger)
    {
        logger.LogInformation("Requesting victual [{VictualId}] for user [{UserId}]", victualId, userId);
        var result = await FetchUserVictualsFromDbAsync(userId, victualId);
        return result;
    }

    public static async Task<List<Victual>> FetchUserVictualsAsync(string userId, ILogger logger)
    {
        logger.LogInformation("Requesting victuals for user [{UserId}]",userId);
        var result = await FetchUserVictualsFromDbAsync(userId);
        return result;
    }

    public static async Task DeleteUserVictualAsync(string userId, string victualId, ILogger logger)
    {
        logger.LogInformation("Deleting victual [{VictualId}] of user [{UserId}]", victualId, userId);
        await DeleteUserVictualFromDbAsync(userId, victualId);
        logger.LogInformation("Delete successful");
    }
    public static async Task CreateVictualAsync(Victual uploadedVictual, ILogger logger)
    {
        logger.LogInformation($"Posting new victual!");
        await PersistToDbAsync(uploadedVictual);
        logger.LogInformation("Posting successful");
    }

    public static async Task UpdateVictualAsync(
        string userId, 
        string victualId, 
        Victual updatedVictual,
        ILogger logger)
    {
        if (!(await ExistsUserVictual(userId, victualId, logger)))
        {
            logger.LogInformation("No victual found to update");
            return;
        }

        logger.LogInformation("Updating victual [{VictualId}] of user[{UserId}]", victualId, userId);
        await PersistToDbAsync(updatedVictual);
        logger.LogInformation("Updating (PUT) successful");
    }

    private static async Task<bool> ExistsUserVictual(string userId, string victualId, ILogger logger)
    {
        return (await FetchUserVictualsAsync(userId, victualId, logger)).Any();
    }
}