namespace Pantrymony.back.Definitions;


public static class Constants
{
    public const string UserIdTag = "userId";
    public const string VictualIdTag = "victualId";
    public const string ImageKeyTag = "imageKey";
    public const string UserEmailClaim = "email";
    public const string RequestHeaderAuthorizationTag = "Authorization";
    
    public static class EnvironmentVariableTags
    {
        public const string BucketNameTag = "IMAGES_S3_BUCKET";
        public const string SignedUrlExpirationTag = "SIGNED_URL_EXPIRATION_MINUTES";
        public const string JwksUrl = "JWKS_URL";
        public const string VictualsTable = "VICTUALS_TABLE";
    }
}

