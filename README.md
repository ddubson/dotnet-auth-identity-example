# .NET Auth Identity Example with IdentityServer4 and .NET Core Identity

This solution demonstrates two OAuth flows that could be of interest for those creating
a service-based architecture with Single Page Applications (SPAs) or Mobile clients. This material
is adopted from IdentityServer4 documentation as well as .NET Architecture repository
`eShopOnContainers`.

The sample APIs are based on Produce (like vegetables, fruits, etc.), just as an example.

Below you'll find two authentication/authorization flows.

1. **Service-to-Service communication** - one microservice wants to access another's endpoints, but endpoints
are protected by an identity server (OAuth) - the `Client Credentials` Grant Type (flow)
2. **Single-Page App (SPA) to API** - i.e. React, Angular, Vue - SPA wants to access downstream API but endpoints are 
protected by an identity server (OAuth) - the [`Code` Grant Type](https://developer.okta.com/blog/2018/04/10/oauth-authorization-code-grant-type)

## Service Blueprint

Here is the layout of the services in this solution

| Service Name | Port | Port Type |
| -------- | -------- | -------- |
| Identity API | 5000     | `https`     |
| Identity API | 5001     | `http`     |
| Produce API | 5005     | `https`     |
| Produce API | 5006     | `http`     |
| React App Client | 5010     | `https`     |
| React App Client | 5011     | `http`     |

## Starting projects locally

Get the .NET Core SDK on MacOS via: `brew cask install dotnet-sdk`

Identity API: 

`dotnet run --project src/Identity.API`

Produce API: 

`dotnet run --project src/Produce.API`

Produce Single Page App (SPA): 

`dotnet run --project src/Produce.SPA`

## Authorization Flow 1: Client Credentials

OAuth 2 Client Credentials flow is geared mostly towards service-to-service communication, since
client id and client secret is embedded in the client as well as the auth service.

[Detailed description of the flow here](https://auth0.com/docs/flows/concepts/client-credentials)

Within this solution, client credential flow is used like so:

Identity.API is up and running on `:5000`,
Produce API is up and running on `:5005`

1. `ClientCredentialsFlow.Client` wants to access the Produce API but the resources are protected
2. `Identity.API` has `Produce.API` registered as a protected resource, and it has [allowed Client Credentials Grant Type (:35)](src\Identity.API\Config.cs) on this API.
All it requires is that connecting clients know `ClientID` and `ClientSecret`
3. `Produce.API` registered `Identity.API` as its identity server, and when a request for a protected resource comes through, the identity server is pinged with proper token.
If not provided, the resource request is rejected.
4. `ClientCredentialsFlow.Client` now turns to `Identity.API` and asks about its endpoints
5. `ClientCredentialsFlow.Client` finds the right endpoint to ask for a Bearer token
6. `ClientCredentialsFlow.Client`, now having a Bearer token, turns to the `Produce.API` and asks for the protected
resource again, but this time with `Authorization: Bearer [token]` in the HTTP request header.
7. `ClientCredentialsFlow.Client` is now able to view the protected resource of the `Produce.API`

## Authorization Flow 2: Code

OAuth 2 Code (Implicit) flow is geared towards single page applications or other
clients that have user navigation and authorization flow.

Within this solution, the code flow is used like so:

> `Identity.API` is up and running on `5000`
>
> `Produce.API` is up and running on `5005`
>
> `Produce.SPA` is up and running on `5010`

- `Produce.SPA` (Single Page App written in React) wants to access `Produce.API` endpoints, but they are protected
via the `Authorize` attribute. `Produce.API` is hooked up to `Identity.API` as its authority on who has access and 
who does not.
- `Identity.API` has a configuration file `Config.cs` which entails OAuth configuration for Authorization Code grant type

```csharp
authorizationCodeFlowClient = new Client
    {
        ClientId = "produce-spa",
        ClientName = "Produce SPA React App",
        RequirePkce = true,
        RequireClientSecret = false,
        AllowedGrantTypes = GrantTypes.Code,
    
        RedirectUris = {"https://localhost:5010/callback"},
        PostLogoutRedirectUris = {"https://localhost:5010"},
        AllowedCorsOrigins = {"https://localhost:5010"},
    
        AllowedScopes =
        {
            "ProduceAPI"
        }
    }
``` 

- `Produce.SPA` needs a Bearer token to talk to `Produce.API`, and it can get it from `Identity.API`, so all it has to
do is ask for a new token (i.e. authorize). In the SPA (Single Page App), this is done via redirect to the Identity
server for login.
- Once the user has logged in on the Identity Server, the user is redirected back to the original SPA, but with a token
in hand.
- The SPA will now handle the token, and is able to make requests on behalf of the user, to the `Produce.API` which
allows the user to fetch its resources.

### How the components are wired up

`Produce.SPA` 

- Has Identity Server hooked up (`Startup.cs`) with `ProduceAPI` resource as its requested resource
- Has CORS policy for itself hooked up (`Startup.cs`)

`Produce.API`

- Has Identity Server JWT bearer (`Startup.cs`) with `ProduceAPI` as its identifier.
- Has CORS policy `Produce.SPA` hooked up (`Startup.cs`)

`Identity.API`

- Has list of APIs it's managing hooked up (`Config.cs:24`)
- Has list of grant types it supports (`Config.cs:32`) -- Code grant type is of interest in this case

---

## Identity API service

Using [IdentityServer4](http://docs.identityserver.io/en/latest/index.html) - OpenID and OAuth2 framework for ASP.NET Core

By default, the database is SQLite and is autogenerated as part of the dotnet template. The file
to reference is `app.db` in the root directory.

```bash
# Run database migrations
dotnet ef database update --project .\src\Identity.API
```

## Creating from scratch

```bash
dotnet new sln --name DotNetAuthIdentityExample

# Install IdentityServer4 templates
dotnet new -i IdentityServer4.Templates

# Create Identity service with IdentityServer4 + ASP.NET Core Identity frameworks
dotnet new is4aspid --name Identity.API --output .\src\Identity.API

# Add Identity service to solution
dotnet sln DotNetAuthIdentityExample.sln add .\src\Identity.API\Identity.API.csproj

# Create an API <-- this is the protected API guarded by the identity service
dotnet new webapi --name ProduceAPI --output .\src\Produce.API

# Add API to solution
dotnet sln DotNetAuthIdentityExample.sln add src\Produce.API

# Add a demonstrational client for Client Credentials flow
dotnet new console --name ClientCredentialsFlow.Client --output .\src\ClientCredentialsFlow.Client
dotnet sln DotNetAuthIdentityExample.sln add src\ClientCredentialsFlow.Client
dotnet add .\src\ClientCredentialsFlow.Client\ClientCredentialsFlow.Client.csproj package IdentityModel

# Add a demonstrational React Single Page App for Implicit flow (aka SPA flow)
dotnet new react --name Produce.SPA --output .\src\Produce.SPA
dotnet sln DotNetAuthIdentityExample.sln add src\Produce.SPA\Produce.SPA.csproj
dotnet add .\src\Produce.SPA\Produce.SPA.csproj package IdentityServer4.AccessTokenValidation
```