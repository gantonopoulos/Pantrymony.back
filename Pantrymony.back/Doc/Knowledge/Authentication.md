# Authentication

## Application Scheme

The frontend authenticates the user by forcing them to log-in through via OAuth0 and an IdP (google).

Upon successful authentication, OAuth0 returns an id_token with the user's email which will be used as the user identifier.

It also returns an access_token for making authenticated requests to the API. 

The access_token will be passed to the request's header and be received by the Authenticator-Lambda.

The Authenticator lambda will manually verify the token and upon successful verification will authorize the source-method execution. 

Note: It is important that I:
    
- specify cors on the OAuth application settings so as to disallow any other client other than my own (I will do this at the very end)
- move the domain to the secrets, otherwise I might be flooded with someone seen my code and authenticated themselves against my OAuth-App, and then started issuing requests through  e.g. postman.

## Token caveats

### Opaque Access_Token

I should only use _access_tokens_ for accessing the API. It is possible to use the _id_token_ as they do in the course, but it is a bad practice. And it also means having to store secrets. 

In order to have a non-opaque access_token issued by OAuth0, I need to define an **audience** in the token request see [here](https://community.auth0.com/t/access-token-is-malformed-jwt-when-authenticating-with-google-oauth2/74218).

For that reason I register my API on OAuth0 and use it as audience. This is the correct thing to do anyway, since my API has to cross check with OAuth if the API-Request it received was issued by the OAuth-App.

### Blazor and symmetrically signed audiences

The API has to be registered as an asymmetrically signed one(RS256), because Blazor could not connect to the OAuth-App while having an HS256 API as audience (No idea why).

This is better because I don't have to store any secrets, as it would be the case if I had symmetric API.

### Access_Token verification

With the _access_token_ being asymmetric, we can verify it if we have the public key counterpart, of the private one used to sign it. 

We can get either by directly issuing a GET request to the URL of the **JSON Web Key Set** found in the **Advanced Settings->Endpoints** section of the OAuth application, or by processing the results of a GET request to the **OpenID Configuration** URL found in the same location (this is what I did). 

### ID_toke verification

The id_token need not be verified, but if we need to, doing so depends on the **JSON Web Token (JWT) Signature Algorithm**. If it is symmetric, we need the secret key set in the main page of the OAuth-App configuration.

If it is asymmetric, we do what we did for the access_token.

### Passing data through the _access_token_ 

If we need to pass any information, e.g. access right, roles etc, from the front end to the API we can use Scopes.

## Sources

[Storing a JWT token on the client side](https://stackoverflow.com/questions/63698112/storing-a-jwt-token-in-blazor-client-side) (Bad Practice)

[Blazor: Json Web Token (JWT) Authentication Example](https://www.prowaretech.com/articles/current/blazor/wasm/jwt-authentication-simple)

[Manually validating a JWT using .NET](https://www.jerriepelser.com/blog/manually-validating-rs256-jwt-dotnet/)

#### About OAuth claims

[Scopes](https://auth0.com/docs/get-started/apis/scopes/openid-connect-scopes)

[JSON Web Token Claims](https://auth0.com/docs/secure/tokens/json-web-tokens/json-web-token-claims)
