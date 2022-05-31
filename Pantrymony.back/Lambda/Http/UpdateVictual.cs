using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Extensions;
using Pantrymony.back.Lambda.Extensions;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda.Http;

public class UpdateVictual
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> UpdateVictualAsync(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var updatedVictual = JsonSerializer.Deserialize<Victual>(request.Body)
                .ThrowIfNull(new InvalidDataException("Malformed request body!"));
            var userId = request.QueryStringParameters[Constants.UserIdTag];
            var victualId = request.QueryStringParameters[Constants.VictualIdTag];
            if (!updatedVictual.UserId.Equals(userId) || !updatedVictual.VictualId.ToString().Equals(victualId))
                throw new ArgumentOutOfRangeException();
            await UserVictualsService.UpdateVictualAsync(userId, victualId, updatedVictual, context.GetCustomLogger());
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse().Log(context.Logger);
    }
}