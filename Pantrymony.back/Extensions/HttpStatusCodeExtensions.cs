using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace Pantrymony.back.Extensions;

public static class HttpStatusCodeExtensions
{
    public static APIGatewayProxyResponse AsApiGatewayProxyResponse(this HttpStatusCode code)
    {
        return new APIGatewayProxyResponse() { StatusCode = (int)code };
    }
    
    public static APIGatewayProxyResponse AsOkGetResponse<T>(this T body) where T:class
    {
        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(body)
        };
    }
    
    public static APIGatewayProxyResponse AsResponse<T>(this T body, HttpStatusCode code) where T:class
    {
        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)code,
            Body = JsonSerializer.Serialize(body)
        };
    }
}