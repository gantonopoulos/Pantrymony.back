using System.Security.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Auth;

public static class TokenOperations
{
    public static string ExtractJwtTokenFromAuthorizationHeader(string authorizationHeader)
    {
        authorizationHeader.ThrowIf(token => 
                !token.ToLower().StartsWith("bearer ") 
                || authorizationHeader.Split(' ').Length != 2, 
            new AuthenticationException($"Malformed authorization token {authorizationHeader}!"));
        return authorizationHeader.Split(' ')[1];
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


    public static async Task<bool> ValidateTokenSignature(string token)
    {
        var handler = new JsonWebTokenHandler();
        var validationResult = handler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = await JwksClient.GetSigningKeyOfToken(token)
        });
        validationResult.Exception.ThrowIf((e) => e is not null, validationResult.Exception);
        return validationResult.IsValid;
    }


    
}