using Amazon.Lambda.Core;
using Pantrymony.back.Model;
using static Pantrymony.back.DataAccess.UserVictualsAccess;

namespace Pantrymony.back.BusinessLogic;

public static class UserVictualsService
{
    public static async Task<List<Victual>> FetchUserVictualsAsync(string userId, string victualId, ILambdaLogger logger)
    {
        logger.LogInformation($"Requesting victual [{victualId}] for user [{userId}]");
        var result = await FetchUserVictualsFromDbAsync(userId, victualId);
        return result;
    }
    
    public static async Task<List<Victual>> FetchUserVictualsAsync(string userId, ILambdaLogger logger)
    {
        logger.LogInformation($"Requesting victuals for user [{userId}]");
        var result = await FetchUserVictualsFromDbAsync(userId);
        return result;
    }

    public static async Task DeleteUserVictualAsync(string userId, string victualId, ILambdaLogger logger)
    {
        logger.LogInformation($"Deleting victual [{victualId}] of user " + $"[{userId}]");
        await DeleteUserVictualFromDbAsync(userId, victualId);
        logger.LogInformation("Delete successful");
    }
    
    public static async Task CreateVictualAsync(Victual uploadedVictual, ILambdaContext context)
    {
        context.Logger.LogInformation($"Posting new victual!");
        await PersistToDbAsync(uploadedVictual);
        context.Logger.LogInformation("Posting successful");
    }
    
    public static async Task UpdateVictualAsync(string userId, string victualId, Victual updatedVictual,
        ILambdaContext context)
    {
        if (!(await ExistsUserVictual(userId, victualId, context)))
        {
            context.Logger.LogInformation("No victual found to update.");
            return;
        }

        context.Logger.LogInformation($"Updating victual [{victualId}] of user[{userId}]");
        await PersistToDbAsync(updatedVictual);
        context.Logger.LogInformation("Updating (PUT) successful");
    }

    private static async Task<bool> ExistsUserVictual(string userId, string victualId, ILambdaContext context)
    {
        return (await FetchUserVictualsAsync(userId, victualId, context.Logger)).Any();
    }
}