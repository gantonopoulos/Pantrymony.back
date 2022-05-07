using System.Net;
using Amazon.Lambda.APIGatewayEvents;

namespace Pantrymony.back.Extensions;

public static class HttpStatusCodeExtensions
{
    public static APIGatewayProxyResponse AsApiGatewayProxyResponse(this HttpStatusCode code)
    {
        return new APIGatewayProxyResponse() { StatusCode = (int)code };
    }
}