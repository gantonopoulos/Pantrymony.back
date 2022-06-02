using Amazon.Lambda.APIGatewayEvents;
using Pantrymony.back.Auth;
using Pantrymony.back.Definitions;

namespace Pantrymony.back.Lambda.Extensions;

public static class HttpRequestExtensions
{
     public static bool WasSentByUser(this APIGatewayProxyRequest request, string userId)
     {
          return TokenOperations.GetTokenClaimValue(
               TokenOperations.ExtractJwtTokenFromAuthorizationHeader(
                    request.Headers[Constants.RequestHeaderAuthorizationTag]),
               Constants.UserEmailClaim).Equals(userId);
     }
}