using IdentityService.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var appConfiguration = builder.Configuration;

builder.Services.AddDbContext<AspNetIdentityDbContext>(options =>
    options.UseSqlServer(defaultConnectionString,
    b => b.MigrationsAssembly(assembly)));

builder.Services.AddAppSettingsConfiguration(appConfiguration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
});

builder.Services.AddIdentityService(defaultConnectionString, assembly);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapDefaultControllerRoute();
app.MapAccountEndpoints(appConfiguration);
app.MapApplicationEndpoints();
app.Run();