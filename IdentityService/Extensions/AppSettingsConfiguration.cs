using IdentityService.Models;

namespace IdentityService.Extensions;

public static class AppSettingsConfiguration
{
    public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RouteSettings>()
            .Bind(configuration.GetSection("RouteSettings"))
            .ValidateDataAnnotations();
        
        services.AddOptions<IdentitySettings>()
            .Bind(configuration.GetSection("IdentitySettings"))
            .ValidateDataAnnotations();
        
        services.AddOptions<IdentityServerSettings>()
            .Bind(configuration.GetSection("IdentityServerSettings"))
            .ValidateDataAnnotations();

        return services;
    }
}