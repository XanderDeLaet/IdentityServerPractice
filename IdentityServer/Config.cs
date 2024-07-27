using System.Collections.Generic;
using Duende.IdentityServer.Models;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource
        {
            Name = "user_roles", // Changed name to "user_roles"
            UserClaims = { ClaimTypes.Role }
        }
            };


        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API")
                {
                    Scopes = { "api1.read", "api1.write" }, // Include "roles" scope here
                    UserClaims = { ClaimTypes.Role } // Ensure roles are included in the API resource
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1.read", "Read access to API 1"),
                new ApiScope("api1.write", "Write access to API 1")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1.read", "api1.write" }
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Ensure the grant type is ResourceOwnerPassword
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1.read", "api1.write", "user_roles" } // Include "roles" scope here
                }
            };
    }
}
