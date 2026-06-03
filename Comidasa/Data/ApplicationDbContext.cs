using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Comidasa.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Comidasa.Models.Product> Products { get; set; }
    public DbSet<Comidasa.Models.Favorite> Favorites { get; set; }
    public DbSet<Comidasa.Models.Review> Reviews { get; set; }
}
