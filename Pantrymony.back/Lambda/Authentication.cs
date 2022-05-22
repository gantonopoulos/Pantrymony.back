using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Security.Cryptography;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
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
        APIGatewayCustomAuthorizerResponse response;
        var accessToken = request.AuthorizationToken.Split(' ')[1];
        context.Logger.LogInformation($"Authenticating user with token :[{accessToken}]");
        
        if (await ValidateTokenSignature(accessToken, context.Logger))
        {
            context.Logger.LogInformation($"Authorized!");
            response = GenerateResponse(GetTokenClaimValue(accessToken, "email"),
                "Allow", 
                request.MethodArn);
        }
        else
        {
            context.Logger.LogInformation($"Denied!");
            response = GenerateResponse(GetTokenClaimValue(accessToken, "email"),
                "Deny", 
                request.MethodArn);
        }
        
        context.Logger.LogInformation($"Generated response: [{JsonSerializer.Serialize(response)}]");
        return response;
    }
   
    private static async Task<bool> ValidateTokenSignature(string token, ILambdaLogger logger)
    {
        try
        {
            JsonWebTokenHandler handler = new JsonWebTokenHandler();
            var validationResult = handler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = await GetSigningKeyOfToken(token)
            });
            validationResult.Exception.ThrowIf((e) => e is not null, validationResult.Exception);
            return validationResult.IsValid;
        }
        catch (Exception ex)
        {
            logger.LogError($"Token validation failed:\n{ex.Message}: {ex.StackTrace}");
            return false;
        }
        
    }

    private static async Task<SecurityKey> GetSigningKeyOfToken(string token)
    {
        JsonWebTokenHandler handler = new JsonWebTokenHandler();
        var publicKeys = await FetchJsonWebKeySet();
        var jsonWebToken = handler.ReadJsonWebToken(token);
        var publicKeyOfToken = publicKeys.Keys.ToList()
            .Where(IsVerificationKey).Single(key => key.Kid == jsonWebToken.Kid);
        
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        RSAParameters rsaParameters = new RSAParameters()
        {
            Modulus = WebEncoders.Base64UrlDecode(publicKeyOfToken.N),
            Exponent = WebEncoders.Base64UrlDecode(publicKeyOfToken.E)
        };
                
        rsa.ImportParameters(rsaParameters);
        return new RsaSecurityKey(rsa);
    }

    private static async Task<JsonWebKeySet> FetchJsonWebKeySet()
    {
        var jwksUrl = Environment.GetEnvironmentVariable("JWKS_URL")
            .ThrowIfNull(new Exception($"Undefined environment variable: [JWKS_URL]!"));
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, jwksUrl);
        var response = await new HttpClient().SendAsync(requestMsg);
        response.ThrowIf(r => !r.IsSuccessStatusCode, 
            new Exception("Could not fetch JWKS from OpenId provider"));
        var publicKeys = JsonWebKeySet.Create(await response.Content.ReadAsStringAsync());
        
        return publicKeys;
    }

    private static bool IsVerificationKey(JsonWebKey jsonWebKey)
    {
        return jsonWebKey.Use == "sig" &&
               jsonWebKey.Alg == "RS256" &&
               jsonWebKey.Kty == "RSA" &&
               !string.IsNullOrEmpty(jsonWebKey.Kid) &&
               jsonWebKey.X5c is not null &&
               jsonWebKey.X5c.Any() &&
               !string.IsNullOrEmpty(jsonWebKey.N) &&
               !string.IsNullOrEmpty(jsonWebKey.E);

    }
    
    private static APIGatewayCustomAuthorizerResponse GenerateResponse(
        string principalId, 
        string effect,
        string resource)
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
            Effect = effect,
            // I should return all accessible lambdas (use "*") or activate caching
            Resource = new HashSet<string>() { "*" }
        };
        policyDocument.Statement.Add(statementOne);
        authResponse.PolicyDocument = policyDocument;
        return authResponse;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="claimName">e.g. "email"</param>
    /// <returns></returns>
    public static string GetTokenClaimValue(string token, string claimName)
    {
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(token);
        return jsonWebToken.GetPayloadValue<string>(claimName);
    }
}