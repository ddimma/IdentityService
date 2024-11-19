namespace IdentityService.Models;

public class IdentityServerSettings
{
    public IdentityResourceSettings IdentityResources { get; set; }
    public ApiScopeSettings ApiScopes { get; set; }
    public ApiResourceSettings ApiResources { get; set; }
    public ClientSettings Clients { get; set; }
}

public class IdentityResourceSettings
{
    public string Role { get; set; }
}

public class ApiScopeSettings
{
    public string Read { get; set; }
    public string Write { get; set; }
}

public class ApiResourceSettings
{
    public string Name { get; set; }
    public string Secret { get; set; }
}

public class ClientSettings
{
    public string AndroidClientId { get; set; }
    public string AndroidClientName { get; set; }
    public int AccessTokenLifetime { get; set; }
    public int AbsoluteRefreshTokenLifetime { get; set; }
}