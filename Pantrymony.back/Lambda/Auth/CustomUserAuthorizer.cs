using System.Text.Json;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Pantrymony.back.Auth;
using Pantrymony.back.Definitions;

namespace Pantrymony.back.Lambda.Auth;

public class CustomUserAuthorizer
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayCustomAuthorizerResponse> AuthorizeUserAsync(
        APIGatewayCustomAuthorizerRequest request, 
        ILambdaContext context)
    {
        try
        {
            var accessToken = TokenOperations.ExtractJwtTokenFromAuthorizationHeader(request.AuthorizationToken);
            var isTokenValidated = await TokenOperations.ValidateTokenSignature(accessToken);
            var userEmail =  TokenOperations.GetTokenClaimValue(accessToken, Constants.UserEmailClaim);
            var response = GenerateResponse(
                userEmail, 
                isTokenValidated
                    ? Statement.StatementEffect.Allow
                    : Statement.StatementEffect.Deny); 
        
            context.Logger.LogInformation($"Generated response: [{JsonSerializer.Serialize(response)}]");
            return response;
        }
        catch (Exception e)
        {
            context.Logger.LogError($"Error {e}\n Stack: {e.StackTrace}");
            return GenerateResponse(Guid.NewGuid().ToString(), Statement.StatementEffect.Deny);
        }
    }

    private APIGatewayCustomAuthorizerResponse GenerateResponse(string principalId, Statement.StatementEffect effect)
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