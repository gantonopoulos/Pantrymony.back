using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.IdentityModel.Tokens.Jwt;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Lambda;

public class Authentication
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public static async Task<APIGatewayCustomAuthorizerResponse> AuthenticateAsync(
        APIGatewayCustomAuthorizerRequest request, 
        ILambdaContext context)
    {
        APIGatewayCustomAuthorizerResponse result;
        var accessToken = request.AuthorizationToken.Split(' ')[1];
        context.Logger.LogInformation($"Authenticating user with token :[{accessToken}]");
        if (await ValidateAccessToken(accessToken, context.Logger))
        {
            context.Logger.LogInformation($"Authorized!");
            result = GenerateResponse("user", "Allow", request.MethodArn);
        }
        else
        {
            context.Logger.LogInformation($"Denied!");
            result = GenerateResponse("user", "Deny", request.MethodArn);
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
    
    private static async Task<bool> ValidateAccessToken(string token, ILambdaLogger logger)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var oauthDomain = Environment.GetEnvironmentVariable("AUTH_DOMAIN")
                .ThrowIfNull(new Exception($"Undefined environment variable: [AUTH_DOMAIN]!"));
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = 
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"https://{oauthDomain}/.well-known/openid-configuration", 
                    new OpenIdConnectConfigurationRetriever());
            
            logger.LogInformation($"Getting signing keys!");
            OpenIdConnectConfiguration openIdConfig = 
                await configurationManager.GetConfigurationAsync(CancellationToken.None);
            var keys = openIdConfig.SigningKeys;
            logger.LogInformation($"Found {keys.Count} keys!");
            
            var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
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
            logger.LogError($"{ex.Message}: {ex.StackTrace}");
            return false;
        }
        return true;
    }
}