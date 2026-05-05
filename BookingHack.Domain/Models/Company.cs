namespace BookingHack.Domain.Models;

public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string TypeOfService { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Company() { }

    public Company(string name, string typeOfService, string address, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        TypeOfService = typeOfService;
        Address = address;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string typeOfService, string address, string? description)
    {
        Name = name;
        TypeOfService = typeOfService;
        Address = address;
        Description = description;
    }
}