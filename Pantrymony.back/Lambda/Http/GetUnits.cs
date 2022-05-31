using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.Extensions;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda.Http;

public class GetUnits
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> GetUnitsAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        context.Logger.LogInformation("Requesting supported units.");
        return await Task.Run(()=> Unit.SupportedUnits.AsOkGetResponse().Log(context.Logger));
    }
    
}