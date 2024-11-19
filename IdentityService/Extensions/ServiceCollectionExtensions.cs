namespace IdentityService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InitializeAppConfig(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        Config.Initialize(configuration);
        return serviceCollection;
    }
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration, string? connectionString, string? assemblyName)
    {
        var identitySettings = new IdentitySettings();
        configuration.GetSection("IdentitySettings").Bind(identitySettings);
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = identitySettings.RequireUniqueEmail;
                options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumericPassword;
                options.Password.RequiredLength = identitySettings.RequiredLengthPassword;
            })
            .AddEntityFrameworkStores<AspNetIdentityDbContext>()    
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(5);
        });
        
        services.AddIdentityServer()
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assemblyName));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assemblyName));
            })
            .AddDeveloperSigningCredential();

        return services;
    }
}