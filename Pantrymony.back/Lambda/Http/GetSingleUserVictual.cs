using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Extensions;
using Pantrymony.back.Lambda.Extensions;

namespace Pantrymony.back.Lambda.Http;

public class GetSingleUserVictual
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    private async Task<APIGatewayProxyResponse> GetSingleUserVictualAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var userId = request.QueryStringParameters[Constants.UserIdTag];
            var victualId = request.QueryStringParameters[Constants.VictualIdTag];
            var result = await UserVictualsService.FetchUserVictualsAsync(userId, victualId, context.GetCustomLogger());
            return result.AsOkGetResponse().Log(context.Logger);
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }
        
    }
}