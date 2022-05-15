using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.IdentityModel.Tokens.Jwt;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
    
    private async Task<bool> ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = 
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"https://gantonopoulos.eu.auth0.com/.well-known/openid-configuration", 
                    new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
            var keys = openIdConfig.SigningKeys;
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = false,
                ValidateTokenReplay = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = openIdConfig.SigningKeys,
            }, out SecurityToken validatedToken);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"{ex.Message}: {ex.StackTrace}");
            return false;
        }
        return true;
    }
}