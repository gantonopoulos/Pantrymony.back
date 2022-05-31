using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace Pantrymony.back.Lambda.Extensions;

public static class HttpStatusCodeExtensions
{
    public static APIGatewayProxyResponse Log(this APIGatewayProxyResponse response, ILambdaLogger logger)
    {
        logger.LogInformation($"Responding with:[{JsonSerializer.Serialize(response)}]");
        return response;
    }
    
    public static APIGatewayProxyResponse AsApiGatewayProxyResponse(this HttpStatusCode code)
    {
        return new APIGatewayProxyResponse() { StatusCode = (int)code }.AllowCors();
    }
    
    public static APIGatewayProxyResponse AsOkGetResponse<T>(this T body) where T:class
    {
        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(body)
        }.AllowCors();
    }
    
    public static APIGatewayProxyResponse AsResponse<T>(this T body, HttpStatusCode code) where T:class
    {
        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)code,
            Body = JsonSerializer.Serialize(body)
        }.AllowCors();
    }

    private static APIGatewayProxyResponse AllowCors(this APIGatewayProxyResponse response)
    {
        response.Headers ??= new Dictionary<string, string>();
        response.Headers.Add("Access-Control-Allow-Headers",
            "Content-Type, Authorization, X-Amz-Date, X-Api-Key, X-Amz-Security-Token");
        response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS, DELETE, GET, HEAD, PATCH, POST, PUT");
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Credentials", "false");
        return response;
    }
}