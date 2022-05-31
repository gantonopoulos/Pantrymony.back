using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.Definitions;
using Pantrymony.back.Extensions;
using static Pantrymony.back.BusinessLogic.StorageService;

namespace Pantrymony.back.Lambda.Storage;

public class PreSignedDownloadUrlRequester
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> RequestPreSignedDownloadUrlAsync(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var imageKey = request.QueryStringParameters[Constants.ImageKeyTag];
            context.Logger.LogInformation($"Requesting Download-Url for key:[{imageKey}]");
            if (await ExistsS3ResourceWithKeyAsync(imageKey))
            {
                return (await RequestSignedUrlAsync(HttpVerb.GET, imageKey)).AsOkGetResponse()
                    .Log(context.Logger);
            }

            return await Task.Run(()=> HttpStatusCode.NotFound.AsApiGatewayProxyResponse().Log(context.Logger));
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }
    }
}