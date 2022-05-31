using Amazon.S3;
using Amazon.S3.Model;
using static Pantrymony.back.Definitions.Constants.EnvironmentVariableTags;

namespace Pantrymony.back.BusinessLogic;

public class StorageService
{
    public static async Task<string> RequestSignedUrlAsync(HttpVerb httpVerb, string imageKey)
    {
        double.TryParse(Environment.GetEnvironmentVariable(SignedUrlExpirationTag),
            out double minutesToUrlExpiration);

        var req = new GetPreSignedUrlRequest()
        {
            BucketName = Environment.GetEnvironmentVariable(BucketNameTag),
            Key = $"{imageKey}",
            Expires = DateTime.Now.AddMinutes(minutesToUrlExpiration),
            Verb = httpVerb
        };
        
        using var client = new AmazonS3Client();
        {
            return await Task.Run(() => client.GetPreSignedURL(req));
        }
    }

    public static async Task<bool> ExistsS3ResourceWithKeyAsync(string key)
    {
        var signedUrl = await RequestSignedUrlAsync(HttpVerb.GET, key);
        using HttpClient httpClient = new HttpClient();
        using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, signedUrl);
        var downloadResponse = await httpClient.SendAsync(downloadRequest, HttpCompletionOption.ResponseContentRead);
        return downloadResponse.IsSuccessStatusCode;
    }
}