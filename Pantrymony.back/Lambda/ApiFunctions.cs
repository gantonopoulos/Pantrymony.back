using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.Extensions;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda;

public class ApiFunctions
{
    private const string BucketNameTag = "IMAGES_S3_BUCKET";
    private const string SignedUrlExpirationTag = "SIGNED_URL_EXPIRATION_MINUTES";

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    private static async Task<APIGatewayProxyResponse> GetUserVictual(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var userId = request.QueryStringParameters["userId"];
            var victualId = request.QueryStringParameters["victualId"];
            var result = await GetUserVictualFromDb(userId, victualId , context.Logger);
            context.Logger.LogInformation($"Found {result.Count()} victuals");
            return result.AsOkGetResponse().Log(context.Logger);
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }
        
    }
    
    private static async Task<List<Victual>> GetUserVictualFromDb(string userId, string victualId, ILambdaLogger logger)
    {
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        logger.LogInformation($"Requesting victual [{victualId}] for user [{userId}]");
        var result = await dbContext.QueryAsync<Victual>(
                userId, QueryOperator.Equal, new[] { victualId })
            .GetRemainingAsync();
        return result;
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetUserVictuals(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        await ValidateTableExistsAsync();
        var dbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        var userId = request.QueryStringParameters["userId"];

        context.Logger.LogInformation($"Requesting victuals for user [{userId}]");
        IEnumerable<Victual> result = await dbContext.QueryAsync<Victual>(userId).GetRemainingAsync();
        context.Logger.LogInformation($"Found {result.Count()} victuals");
        return result.AsOkGetResponse().Log(context.Logger);
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
            AWSSDKHandler.RegisterXRayForAllServices();
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
            AWSSDKHandler.RegisterXRayForAllServices();
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
            AWSSDKHandler.RegisterXRayForAllServices();
            await ValidateTableExistsAsync();
            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            
            
            context.Logger.LogInformation(
                $"Updating victual [{request.QueryStringParameters["victualId"]}] of " +
                $"user[{request.QueryStringParameters["userId"]}] to:\n" +
                $"{request.Body}");
            var newVictual = JsonSerializer.Deserialize<Victual>(request.Body);
            var storedVictual = await dbContext.LoadAsync<Victual>(
                request.QueryStringParameters["userId"],
                request.QueryStringParameters["victualId"]);

            if (storedVictual == null)
            {
                context.Logger.LogInformation("No victual found to update.");
                return HttpStatusCode.NotFound.AsApiGatewayProxyResponse().Log(context.Logger);
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
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse().Log(context.Logger);
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetUnits(APIGatewayProxyRequest request, ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        context.Logger.LogInformation("Requesting supported units.");
        return await Task.Run(()=> Unit.SupportedUnits.AsOkGetResponse().Log(context.Logger));
    }
    
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> Options(APIGatewayProxyRequest request, ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        context.Logger.LogInformation("Options Request arrived!");
        return await Task.Run(()=> HttpStatusCode.OK.AsApiGatewayProxyResponse().Log(context.Logger));
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetSignedUploadUrl(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            return (await RequestSignedUrl(HttpVerb.PUT, request.QueryStringParameters["imageKey"]))
                .AsOkGetResponse().Log(context.Logger);
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }
    }
    
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetSignedDeleteUrl(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var imageKey = request.QueryStringParameters["imageKey"];
            if (await ExistsS3ResourceWithKeyAsync(imageKey, context.Logger))
            {
                return (await RequestSignedUrl(HttpVerb.DELETE, imageKey)).AsOkGetResponse()
                    .Log(context.Logger);
            }

            return await Task.Run(()=> HttpStatusCode.NotFound.AsApiGatewayProxyResponse().Log(context.Logger));
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }
    }

    private static async Task<bool> ExistsS3ResourceWithKeyAsync(string imageKey, ILambdaLogger logger)
    {
        var signedUrl = await RequestSignedUrl(HttpVerb.GET, imageKey);
        using HttpClient httpClient = new HttpClient();
        using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, signedUrl);
        var downloadResponse = await httpClient.SendAsync(downloadRequest, HttpCompletionOption.ResponseContentRead);
        return downloadResponse.IsSuccessStatusCode;
    }

    private static async Task<string> RequestSignedUrl(HttpVerb httpVerb, string imageKey)
    {
        double.TryParse(Environment.GetEnvironmentVariable(SignedUrlExpirationTag), out double minutesToUrlExpiration);

        var req = new GetPreSignedUrlRequest()
        {
            BucketName = Environment.GetEnvironmentVariable(BucketNameTag),
            Key = $"{imageKey}",
            Expires = DateTime.Now.AddMinutes(minutesToUrlExpiration),
            Verb = httpVerb
        };
        
        using var client = new AmazonS3Client();
        {
            return await Task.Run(() => client.GetPreSignedURL(req));
        }
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayProxyResponse> GetSignedDownloadUrl(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var imageKey = request.QueryStringParameters["imageKey"];
            context.Logger.LogInformation($"Requesting Download-Url for key:[{imageKey}]");
            if (await ExistsS3ResourceWithKeyAsync(imageKey, context.Logger))
            {
                return (await RequestSignedUrl(HttpVerb.GET, imageKey)).AsOkGetResponse()
                    .Log(context.Logger);
            }

            return await Task.Run(()=> HttpStatusCode.NotFound.AsApiGatewayProxyResponse().Log(context.Logger));
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }
    }
}