using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pantrymony.back.Extensions;

namespace Pantrymony.back.Auth;

internal static class JwksClient
{
    public static async Task<SecurityKey> GetSigningKeyOfToken(string token)
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
}