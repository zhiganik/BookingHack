using System.Security.Cryptography;
using System.Text;

namespace BookingHack.Domain.Models;

public static class RefreshToken
{
    public static string GenerateRaw() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public static string Hash(string rawToken) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
}
