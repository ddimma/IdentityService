namespace IdentityService.Data;

public class AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options);