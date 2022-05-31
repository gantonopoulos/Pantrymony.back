using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Pantrymony.back.Extensions;
using Pantrymony.back.Model;
using static Pantrymony.back.Definitions.Constants.EnvironmentVariableTags;

namespace Pantrymony.back.DataAccess;

public static class UserVictualsAccess
{
    public static async Task<List<Victual>> FetchUserVictualsFromDbAsync(string userId, string victualId)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        var result = await dbContext.QueryAsync<Victual>(
                userId, QueryOperator.Equal, new[] { victualId })
            .GetRemainingAsync();
        return result;
    }

    public static async Task<List<Victual>> FetchUserVictualsFromDbAsync(string userId)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        return await dbContext.QueryAsync<Victual>(userId).GetRemainingAsync();
    }

    private static async Task ValidateTableExistsAsync()
    {
        try
        {
            var tableName = Environment.GetEnvironmentVariable(VictualsTable)
                .ThrowIfNull(new Exception($"Undefined environment variable {VictualsTable}!"));
            var client = new AmazonDynamoDBClient();
            await client.DescribeTableAsync($"{tableName}");
        }
        catch (ResourceNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task DeleteUserVictualFromDbAsync(string userId, string victualId)
    {
        await ValidateTableExistsAsync();
        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        await dbContext.DeleteAsync<Victual>(userId, victualId);
    }

    public static async Task PersistToDbAsync(Victual uploadedVictual)
    {
        await ValidateTableExistsAsync();
        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        await dbContext.SaveAsync(uploadedVictual);
    }
}