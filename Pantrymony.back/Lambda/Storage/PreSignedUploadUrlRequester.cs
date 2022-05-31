using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Pantrymony.back.BusinessLogic;
using Pantrymony.back.Definitions;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Lambda.Storage;

public class PreSignedUploadUrlRequester
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> RequestPreSignedUploadUrlAsync(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        try
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            return (await StorageService.RequestSignedUrlAsync(
                    HttpVerb.PUT,
                    request.QueryStringParameters[Constants.ImageKeyTag]))
                .AsOkGetResponse().Log(context.Logger);
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return e.Message.AsResponse(HttpStatusCode.BadRequest).Log(context.Logger);
        }
    }
}