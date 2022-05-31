using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Lambda.Http;

public class GetManyUserVictuals
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> GetManyUserVictualsAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        var userId = request.QueryStringParameters[Constants.UserIdTag];
        var result = await UserVictualsService.FetchUserVictualsAsync(userId, context.Logger);
        context.Logger.LogInformation($"Found {result.Count()} victuals");
        return result.AsOkGetResponse().Log(context.Logger);
    }
}