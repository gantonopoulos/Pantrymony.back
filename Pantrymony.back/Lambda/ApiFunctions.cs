using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
        const string victualsTable = "VICTUALS_TABLE";
        var tableName = Environment.GetEnvironmentVariable(victualsTable)
            .ThrowIfNull(new Exception($"Undefined environment variable {victualsTable}!"));
        ValidateTableExists(tableName);

        var client = new AmazonDynamoDBClient();
        var dbContext = new DynamoDBContext(client);
        return await dbContext.ScanAsync<Victual>(Enumerable.Empty<ScanCondition>()).GetRemainingAsync();
    }

    private static async void ValidateTableExists(string envTables)
    {
        try
        {
            var client = new AmazonDynamoDBClient();
            await client.DescribeTableAsync($"{envTables}");
        }
        catch (ResourceNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static APIGatewayProxyResponse DeleteVictual(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {
        var pathParams = apigProxyEvent.PathParameters
            ?.Aggregate("", (res, kvp) => res + $",[{kvp.Key}:{kvp.Value}]");        
        var queryParams = apigProxyEvent.QueryStringParameters
            ?.Aggregate("", (res, kvp) => res + $",[{kvp.Key}:{kvp.Value}]");
        Console.WriteLine($"Got Body[[{apigProxyEvent}]" +
                          $"[{apigProxyEvent.Headers}]]" +
                          $"[{pathParams}]" +
                          $"[{queryParams}]");
        Guid id = Guid.Empty;
        if (DataSource.Data.All(v => v.Identifier != id))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 404,
            };
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(v => v.Identifier == id));
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 202,
        };
    }
}