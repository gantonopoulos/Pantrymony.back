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
    public static async Task<APIGatewayProxyResponse> GetVictuals(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());

        context.Logger.LogInformation($"Requesting all victuals.");
        IEnumerable<Victual> result = await dbContext.ScanAsync<Victual>(Enumerable.Empty<ScanCondition>()).GetRemainingAsync();
        context.Logger.LogInformation($"Found {result.Count()} victuals");
        return result.AsOkGetResponse();
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    private static async Task<IEnumerable<Victual>> GetUserVictual(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        var userId = request.QueryStringParameters["userId"];
        var victualId = request.QueryStringParameters["victualId"];

        context.Logger.LogInformation($"Requesting victual [{victualId}] for user [{userId}]");
        var result = await dbContext.QueryAsync<Victual>(
                userId, QueryOperator.Equal, new[] { victualId })
            .GetRemainingAsync();
        context.Logger.LogInformation($"Found {result.Count()} victuals");
        return result;
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetUserVictuals(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        var userId = request.QueryStringParameters["userId"];

        context.Logger.LogInformation($"Requesting victuals for user [{userId}]");
        IEnumerable<Victual> result = await dbContext.QueryAsync<Victual>(userId).GetRemainingAsync();
        context.Logger.LogInformation($"Found {result.Count()} victuals");
        return result.AsOkGetResponse();
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
            context.Logger.LogInformation($"Deleting victual [{request.QueryStringParameters["victualId"]}] of user" +
                                          $"[{request.QueryStringParameters["userId"]}]");
            await dbContext.DeleteAsync<Victual>(
                request.QueryStringParameters["userId"],
                request.QueryStringParameters["victualId"]);
            context.Logger.LogInformation("Delete successful");
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
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
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
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
            
            
            context.Logger.LogInformation(
                $"Updating victual [{request.QueryStringParameters["victualId"]}] of user[{request.QueryStringParameters["userId"]}]");
            var newVictual = JsonSerializer.Deserialize<Victual>(request.Body);
            var storedVictual = await dbContext.LoadAsync<Victual>(
                request.QueryStringParameters["userId"],
                request.QueryStringParameters["victualId"]);

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
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse();
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetUnits(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Requesting supported units.");
        return await Task.Run(()=> Unit.SupportedUnits.AsOkGetResponse());
    }
}