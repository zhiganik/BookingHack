using BookingHack.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingHack.Infrastructure.PostgreSql.DbContext;

public class BookingHackDbContext : IdentityDbContext<ApplicationUser>
{
    public BookingHackDbContext(DbContextOptions<BookingHackDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingHackDbContext).Assembly);
    }
}
