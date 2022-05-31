using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Extensions;
using Pantrymony.back.Lambda.Extensions;
using Pantrymony.back.Model;

namespace Pantrymony.back.Lambda.Http;

public class CreateVictual
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> CreateVictualAsync(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            await UserVictualsService.CreateVictualAsync(
                JsonSerializer.Deserialize<Victual>(request.Body)
                    .ThrowIfNull(new InvalidDataException("Malformed request body!")), context.GetCustomLogger());
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest);
        }

        return HttpStatusCode.Created.AsApiGatewayProxyResponse();
    }

}