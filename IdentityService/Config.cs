namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new()
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [new("API.read"), new("API.write")];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new("API")
            {
                Scopes = new List<string> { "API.read", "API.write" },
                ApiSecrets = new List<Secret> { new("ScopeSecret".Sha256()) },
                UserClaims = new List<string> { "role" }
            }
        ];

        public static IEnumerable<Client> Clients =>
        [
            new()
            {
                    ClientId = "android_webapi",
                    ClientName = "Andoid",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,

                    //AllowedCorsOrigins = { "https://localhost:7120" },

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
        ];
    }
}