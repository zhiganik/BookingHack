using BookingHack.Domain.Enums;

namespace BookingHack.Domain.Models;

public sealed class CompanyMember
{
    public string UserId { get; private set; }
    public Guid CompanyId { get; private set; }

    public ApplicationUser User { get; private set; }
    public Company Company { get; private set; }

    public CompanyRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    public void UpdateRole(CompanyRole role) => Role = role;
    
    public CompanyMember(string userId, Guid companyId, CompanyRole role)
    {
        if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

        UserId = userId;
        CompanyId = companyId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
    }
}