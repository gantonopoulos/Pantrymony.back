using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda;

public class ApiFunctions
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static IEnumerable<Victual> GetVictuals(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {
        return src.ApiFunctions.GetVictuals();
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