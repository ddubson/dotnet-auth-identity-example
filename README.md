# .NET Auth Identity Example

```bash
dotnet new sln --name DotNetAuthIdentityExample
dotnet new webapp --auth Individual --name Identity.API --output .\src\Identity.API
dotnet sln DotNetAuthIdentityExample.sln add .\src\Identity.API\Identity.API.csproj
```

## Identity service

Default ports: https `5005`, http `5006`

Endpoints exposed by `ASP.NET Core Identity` framework:

```
/Identity/Account/Login
/Identity/Account/Logout
/Identity/Account/Manage
```

By default, the database is SQLite and is autogenerated as part of the dotnet template. The file
to reference is `app.db` in the root directory.

```bash
# Run database migrations
dotnet ef database update --project .\src\Identity.API
```