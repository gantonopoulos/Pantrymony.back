using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Lambda.ApiGateway;

public class PreflightRequestHandler
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> HandlePreflightRequestAsync(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        context.Logger.LogInformation("Options Request arrived!");
        return await Task.Run(()=> HttpStatusCode.OK.AsApiGatewayProxyResponse().Log(context.Logger));
    }
}