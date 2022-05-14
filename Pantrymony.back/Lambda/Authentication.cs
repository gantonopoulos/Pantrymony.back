using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.IdentityModel.Tokens;

namespace Pantrymony.back.Lambda;

public class Authentication
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayCustomAuthorizerResponse> AuthenticateAsync(
        APIGatewayCustomAuthorizerRequest request, 
        ILambdaContext context)
    {
        APIGatewayCustomAuthorizerResponse result;
        var authenticationToken =request.AuthorizationToken.Split(' ')[1];
        context.Logger.LogInformation($"Authenticating user with token :[{ authenticationToken}]");
        switch (authenticationToken)
        {
            case "Allow":
                context.Logger.LogInformation($"Authorised!");
                result = GenerateResponse("user", "Allow", request.MethodArn);
                break;
            case "Deny":
                context.Logger.LogInformation($"Denied!");
                result = GenerateResponse("user", "Deny", request.MethodArn);
                break;                
            case "Unauthorized":
                throw new UnauthorizedAccessException();
            default:
                throw new SecurityTokenValidationException();
        }
        context.Logger.LogInformation($"Generated response: [{JsonSerializer.Serialize(result)}]");
        return result;
    }

    private static APIGatewayCustomAuthorizerResponse GenerateResponse(string principalId, string effect,
        string resource)
    {
        var authResponse = new APIGatewayCustomAuthorizerResponse();
        
        authResponse.PrincipalID = principalId;
        var policyDocument = new APIGatewayCustomAuthorizerPolicy();
        policyDocument.Version = "2012-10-17";
        policyDocument.Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>();
        var statementOne = new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement();
        statementOne.Action = new HashSet<string>(){"execute-api:Invoke"};
        statementOne.Effect = effect;
        // I should return all accessible lambdas (use "*") or activate caching
        statementOne.Resource = new HashSet<string>() { resource };
        policyDocument.Statement.Add(statementOne);
        authResponse.PolicyDocument = policyDocument;
        return authResponse;
    }
}