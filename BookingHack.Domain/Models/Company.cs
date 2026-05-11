using BookingHack.Domain.Enums;

namespace BookingHack.Domain.Models;

public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string TypeOfService { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<CompanyMember> Members { get; private set; }

    private Company() { }
    
    public Company(string name, string typeOfService, string address, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        TypeOfService = typeOfService;
        Address = address;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Members =  new HashSet<CompanyMember>();
    }

    public void Update(string name, string typeOfService, string address, string? description)
    {
        Name = name;
        TypeOfService = typeOfService;
        Address = address;
        Description = description;
    }
    
    public void AddMember(string userId, CompanyRole role)
    {
        if (Members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already a member of this company.");

        if (role == CompanyRole.Owner && Members.Any(m => m.Role == CompanyRole.Owner))
            throw new InvalidOperationException("Company already has an owner.");

        Members.Add(new CompanyMember(userId, Id, role));
    }

    public void RemoveMember(string userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new InvalidOperationException("User is not a member of this company.");
        Members.Remove(member);
    }

    public void UpdateMemberRole(string userId, CompanyRole newRole)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new InvalidOperationException("User is not a member of this company.");
        member.UpdateRole(newRole);
    }
}