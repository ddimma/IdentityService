using IdentityService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
    public class AspNetIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options) 
            : base (options) 
        {
        }
    }
}
