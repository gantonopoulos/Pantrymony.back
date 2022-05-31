using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.Definitions;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Lambda.Extensions;

namespace Pantrymony.back.Lambda.Storage;

public class PreSignedDeleteUrlRequester
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> RequestPreSignedDeleteUrlAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var imageKey = request.QueryStringParameters[Constants.ImageKeyTag];
            if (await StorageService.ExistsS3ResourceWithKeyAsync(imageKey))
            {
                return (await StorageService.RequestSignedUrlAsync(HttpVerb.DELETE, imageKey)).AsOkGetResponse()
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