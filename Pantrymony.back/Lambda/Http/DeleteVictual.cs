using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Lambda.Extensions;

namespace Pantrymony.back.Lambda.Http;

public class DeleteVictual
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> DeleteVictualAsync(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var victualId = request.QueryStringParameters[Constants.VictualIdTag];
            var userId = request.QueryStringParameters[Constants.UserIdTag];
            if (!request.WasSentByUser(userId))
            {
                return HttpStatusCode.Unauthorized.AsApiGatewayProxyResponse();
            }
            await UserVictualsService.DeleteUserVictualAsync(userId, victualId, context.GetCustomLogger());
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }

        return HttpStatusCode.Accepted.AsApiGatewayProxyResponse();
    }
}