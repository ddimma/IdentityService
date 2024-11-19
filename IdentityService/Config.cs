namespace IdentityService;

public static class Config
{
    private static IdentityServerSettings _identityServerSettings;

    public static void Initialize(IConfiguration configuration)
    {
        var identityServerSettings = configuration.GetSection("IdentityServerSettings").Get<IdentityServerSettings>();
        _identityServerSettings = identityServerSettings ?? throw new InvalidOperationException("Failed to bind IdentityServerSettings. Please ensure the configuration section is present and valid in appsettings.json.");
    }

    public static IEnumerable<IdentityResource> IdentityResources => new[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource
        {
            Name = _identityServerSettings.IdentityResources.Role,
            UserClaims = new List<string> { _identityServerSettings.IdentityResources.Role }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope(_identityServerSettings.ApiScopes.Read),
        new ApiScope(_identityServerSettings.ApiScopes.Write)
    };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource(_identityServerSettings.ApiResources.Name)
        {
            Scopes = new List<string> { _identityServerSettings.ApiScopes.Read, _identityServerSettings.ApiScopes.Write },
            ApiSecrets = new List<Secret> { new Secret(_identityServerSettings.ApiResources.Secret.Sha256()) },
            UserClaims = new List<string> { _identityServerSettings.IdentityResources.Role }
        }
    };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = _identityServerSettings.Clients.AndroidClientId,
            ClientName = _identityServerSettings.Clients.AndroidClientName,
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            RequireClientSecret = false,
            AllowOfflineAccess = true,
            AccessTokenLifetime = _identityServerSettings.Clients.AccessTokenLifetime,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            RefreshTokenExpiration = TokenExpiration.Absolute,
            AbsoluteRefreshTokenLifetime = _identityServerSettings.Clients.AbsoluteRefreshTokenLifetime,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                _identityServerSettings.ApiScopes.Read,
                _identityServerSettings.ApiScopes.Write,
                _identityServerSettings.IdentityResources.Role
            },
            AllowAccessTokensViaBrowser = true,
            RequireConsent = false
        }
    };
}