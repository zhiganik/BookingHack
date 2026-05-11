using BookingHack.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingHack.Infrastructure.PostgreSql.Configuration;

public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.ToTable("company_members");

        builder.HasKey(cm => new { cm.UserId, cm.CompanyId });

        builder.Property(cm => cm.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(cm => cm.CompanyId).HasColumnName("company_id").IsRequired();
        builder.Property(cm => cm.Role).HasColumnName("role").HasConversion<string>().IsRequired();
        builder.Property(cm => cm.JoinedAt).HasColumnName("joined_at");

        builder.HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Company)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
