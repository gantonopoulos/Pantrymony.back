using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Pantrymony.back.Extensions;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda;

public class ApiFunctions
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<IEnumerable<Victual>> GetVictuals(APIGatewayProxyRequest _, ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        return await dbContext.ScanAsync<Victual>(Enumerable.Empty<ScanCondition>()).GetRemainingAsync();
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<IEnumerable<Victual>> GetVictualsOfUser(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        context.Logger.LogLine($"Requesting victuals for user [{request.PathParameters["userId"]}]");
        var victuals = await dbContext.QueryAsync<Victual>(request.PathParameters["userId"]).GetRemainingAsync();
        context.Logger.LogLine($"Found {victuals.Count} victuals");
        return victuals;
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<Victual?> GetVictual(APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        var victuals = await dbContext.QueryAsync<Victual>(request.PathParameters["userId"],
            QueryOperator.Equal,
            new[] { request.PathParameters["victualId"] }).GetRemainingAsync();
        return victuals.SingleOrDefault();
    }


    private static async Task ValidateTableExistsAsync()
    {
        try
        {
            const string victualsTable = "VICTUALS_TABLE";
            var tableName = Environment.GetEnvironmentVariable(victualsTable)
                .ThrowIfNull(new Exception($"Undefined environment variable {victualsTable}!"));
            var client = new AmazonDynamoDBClient();
            await client.DescribeTableAsync($"{tableName}");
        }
        catch (ResourceNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> DeleteVictual(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            await ValidateTableExistsAsync();
            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            context.Logger.LogInformation($"Deleting victual [{request.PathParameters["victualId"]}] of user" +
                                          $"[{request.PathParameters["userId"]}]");
            await dbContext.DeleteAsync<Victual>(
                request.PathParameters["userId"],
                request.PathParameters["victualId"]);
            context.Logger.LogInformation("Delete successful");
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return HttpStatusCode.BadRequest.AsApiGatewayProxyResponse();
        }

        return HttpStatusCode.Accepted.AsApiGatewayProxyResponse();
    }
    
    
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> PostVictual(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            await ValidateTableExistsAsync();
            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            
            
            context.Logger.LogInformation($"Posting new victual [{request.Body}]");
            var newVictual = JsonSerializer.Deserialize<Victual>(request.Body);
            await dbContext.SaveAsync(newVictual);
            context.Logger.LogInformation("Posting successful");
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return HttpStatusCode.BadRequest.AsApiGatewayProxyResponse();
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse();
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> PutVictual(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            await ValidateTableExistsAsync();
            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            
            
            context.Logger.LogInformation($"Updating victual " +
                                          $"[{request.PathParameters["victualId"]}] of user" +
                                          $"[{request.PathParameters["userId"]}]");
            var newVictual = JsonSerializer.Deserialize<Victual>(request.Body);
            var storedVictual = await dbContext.LoadAsync<Victual>(
                request.PathParameters["userId"],
                request.PathParameters["victualId"]);

            if (storedVictual == null)
            {
                context.Logger.LogInformation("No victual found to update.");
                return HttpStatusCode.NotFound.AsApiGatewayProxyResponse();
            }
            
            if (newVictual == null || 
                !newVictual.UserId.Equals(storedVictual.UserId) ||
                !newVictual.VictualId.Equals(storedVictual.VictualId))
                throw new ArgumentOutOfRangeException();
            
            await dbContext.SaveAsync(newVictual);
            context.Logger.LogInformation("Updating (PUT) successful");
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return HttpStatusCode.BadRequest.AsApiGatewayProxyResponse();
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse();
    }
}