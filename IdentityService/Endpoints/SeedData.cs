using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.EntityFramework.Storage;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Endpoints
{
    public static class SeedData
    {
        public static void SeedDataEndpoints(this IEndpointRouteBuilder routes)
        {
            var app = routes.MapGroup("/Seed");

            app.MapGet("", (IConfiguration configuration, IServiceScopeFactory scopeFactory) =>
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
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                    }
                );

                services.AddConfigurationDbContext(
                    options =>
                    {
                        options.ConfigureDbContext = db =>
                        db.UseSqlServer(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                    }
                );

                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();

                EnsureSeedData(context);

                var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
                ctx.Database.Migrate();

                return Results.Ok("Seed data process completed successfully.");
            });
        }
        private static void EnsureSeedData(ConfigurationDbContext context)
        {

            Console.WriteLine("Clients being populated");
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
                Console.WriteLine("IdentityResources being populated");
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
}
