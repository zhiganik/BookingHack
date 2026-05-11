namespace BookingHack.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<string?> GetUserIdAsync(string tokenHash);
    Task StoreAsync(string tokenHash, string userId, TimeSpan expiry);
    Task DeleteAsync(string tokenHash);
    Task RevokeAllForUserAsync(string userId);
}
