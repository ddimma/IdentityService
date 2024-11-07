using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };

        public static IEnumerable<ApiScope> ApiScopes => 
            new[] { new ApiScope("API.read"), new ApiScope("API.write"), };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("API")
            {
                Scopes = new List<string> {"API.read", "API.write"},
                ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "android_webapi",
                    ClientName = "Andoid",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,

                    AllowedCorsOrigins = { "https://localhost:7120" },

                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 1800,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 2592000,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "API.read",
                        "API.write",
                        "role"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false
                }
            };
    }
}