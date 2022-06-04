using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MANU_AUTO.Data;

public class MANU_AUTO_IdentityContext : IdentityDbContext<IdentityUser>
{
    public MANU_AUTO_IdentityContext(DbContextOptions<MANU_AUTO_IdentityContext> options)
        : base(options)
    {
    }

    protected MANU_AUTO_IdentityContext(DbContextOptions options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
