using System.Security.Cryptography;
using System.Text;

namespace BookingHack.Domain.Models;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { }

    public static (RefreshToken entity, string rawToken) Create(string userId, int expiryDays = 7)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return (new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = Hash(rawToken),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow
        }, rawToken);
    }

    public static string Hash(string rawToken) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
