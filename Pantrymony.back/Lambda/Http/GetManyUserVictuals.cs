using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Lambda.Extensions;

namespace Pantrymony.back.Lambda.Http;

public class GetManyUserVictuals
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> GetManyUserVictualsAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var userId = request.QueryStringParameters[Constants.UserIdTag];
            if (!request.WasSentByUser(userId))
            {
                return HttpStatusCode.Unauthorized.AsApiGatewayProxyResponse();
            }
            var result = await UserVictualsService.FetchUserVictualsAsync(userId, context.GetCustomLogger());
            context.Logger.LogInformation($"Found {result.Count()} victuals");
            return result.AsOkGetResponse().Log(context.Logger);
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }
    }
}