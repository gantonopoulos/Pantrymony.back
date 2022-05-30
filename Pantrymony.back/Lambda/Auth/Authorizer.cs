using System.Security.Authentication;
using System.Text.Json;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Pantrymony.back.Auth;

namespace Pantrymony.back.Lambda.Auth;

public class Authorizer
{
    private const string UserEmailClaim = "email";

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayCustomAuthorizerResponse> AuthorizeAsync(
        APIGatewayCustomAuthorizerRequest request, 
        ILambdaContext context)
    {
        try
        {
            var accessToken = TokenOperations.ExtractTokenFromRequest(request);
            context.Logger.LogInformation($"Authenticating user with token :[{accessToken}]");
            var isTokenValidated = await TokenOperations.ValidateTokenSignature(accessToken);
            var userEmail =  TokenOperations.GetTokenClaimValue(accessToken, UserEmailClaim);
            var response = GenerateResponse(
                userEmail, 
                isTokenValidated
                    ? Statement.StatementEffect.Allow
                    : Statement.StatementEffect.Deny); 
        
            context.Logger.LogInformation($"Generated response: [{JsonSerializer.Serialize(response)}]");
            return response;
        }
        catch (AuthenticationException e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return GenerateResponse(Guid.NewGuid().ToString(), Statement.StatementEffect.Deny);
        }
    }

    private static APIGatewayCustomAuthorizerResponse GenerateResponse(
        string principalId,
        Statement.StatementEffect effect)
    {
        var authResponse = new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID = principalId
        };

        var policyDocument = new APIGatewayCustomAuthorizerPolicy
        {
            Version = "2012-10-17",
            Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>()
        };

        var statementOne = new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
        {
            Action = new HashSet<string>(){"execute-api:Invoke"},
            Effect = effect.ToString(),
            // I should return all accessible lambdas (use "*") or deactivate caching
            Resource = new HashSet<string>() { "*" }
        };
        policyDocument.Statement.Add(statementOne);
        authResponse.PolicyDocument = policyDocument;
        return authResponse;
    }
}