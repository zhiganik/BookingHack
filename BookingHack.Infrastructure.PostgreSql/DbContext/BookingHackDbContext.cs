using BookingHack.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingHack.Infrastructure.PostgreSql.DbContext;

public class BookingHackDbContext : IdentityDbContext<ApplicationUser>
{
    public BookingHackDbContext(DbContextOptions<BookingHackDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingHackDbContext).Assembly);
    }
}
