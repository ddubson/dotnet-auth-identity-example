// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace Identity.API
{
    public static class Config
    {
        private static Client authorizationCodeFlowClient;
        private static Client clientCredentialsClient;

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("ProduceAPI", "Protected Produce API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
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
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, 
                    "ProduceAPI"
                }
            };

            clientCredentialsClient = new Client
            {
                ClientId = "client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                AllowedScopes = {"ProduceAPI"}
            };

            return new[] {clientCredentialsClient, authorizationCodeFlowClient};
        }
    }
}