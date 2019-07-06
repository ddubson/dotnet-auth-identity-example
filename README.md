# .NET Auth Identity Example

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

## Service Blueprint

| Service Name | Port | Port Type |
| -------- | -------- | -------- |
| Identity API | 5000     | `http`     |
| Identity API | 5000     | `https`     |
| Produce API | 5005     | `http`     |
| Produce API | 5006     | `https`     |
| React App Client | 5010     | `http`     |
| React App Client | 5011     | `https`     |

## Authorization Flow 1: Client Credentials

OAuth 2 Client Credentials flow is geared mostly towards service-to-service communication, since
client id and client secret is embedded in the client as well as the auth service.

[Detailed description of the flow here](https://auth0.com/docs/flows/concepts/client-credentials)

Within this solution, client credential flow is used like so:

Identity.API is up and running on `:5000`,
Produce API is up and running on `:5005, :5006`

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

`Identity.API` is up and running on `5000`,
`Produce.SPA` is up and running on `5010`

1. `Produce.SPA` (Single Page App written in React)

TBD

## Identity API service

Using [IdentityServer4](http://docs.identityserver.io/en/latest/index.html) - OpenID and OAuth2 framework for ASP.NET Core

By default, the database is SQLite and is autogenerated as part of the dotnet template. The file
to reference is `app.db` in the root directory.

```bash
# Run database migrations
dotnet ef database update --project .\src\Identity.API
```
