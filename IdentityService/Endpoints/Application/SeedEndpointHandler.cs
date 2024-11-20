namespace IdentityService.Endpoints.Application;

public static class SeedEndpointHandler
{
    public static async Task<IResult> Seed(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            return Results.BadRequest("Connection string is not configured.");
        }

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AspNetIdentityDbContext>(
            options => options.UseSqlServer(connectionString)
        );

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AspNetIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddOperationalDbContext(
            options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(
                        connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedEndpointHandler).Assembly.FullName)
                    );
            }
        );

        services.AddConfigurationDbContext(
            options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(
                        connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedEndpointHandler).Assembly.FullName)
                    );
            }
        );

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await scope.ServiceProvider.GetService<PersistedGrantDbContext>()?.Database.MigrateAsync()!;

        var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        await context?.Database.MigrateAsync()!;

        EnsureSeedData(context);

        var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
        await ctx?.Database.MigrateAsync()!;

        return Results.Ok("Seed data process completed successfully.");
    }

    private static void EnsureSeedData(ConfigurationDbContext context)
    {
        Console.WriteLine("Clients being populated.");
        
        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            Console.WriteLine("IdentityResources being populated.");
            
            foreach (var resource in Config.IdentityResources.ToList())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in Config.ApiScopes.ToList())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var resource in Config.ApiResources.ToList())
            {
                context.ApiResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
    }
}