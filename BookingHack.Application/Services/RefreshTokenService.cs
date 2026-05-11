using BookingHack.Application.Repositories;
using BookingHack.Domain.Models;

namespace BookingHack.Application.Services;

public class RefreshTokenService
{
    private static readonly TimeSpan TokenExpiry = TimeSpan.FromDays(7);

    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(IRefreshTokenRepository repository)
        => _repository = repository;

    public async Task<string> CreateAsync(string userId)
    {
        var rawToken = RefreshToken.GenerateRaw();
        await _repository.StoreAsync(RefreshToken.Hash(rawToken), userId, TokenExpiry);
        return rawToken;
    }

    public async Task<(string userId, string newRawToken)?> ValidateAndRotateAsync(string rawToken)
    {
        var hash = RefreshToken.Hash(rawToken);
        var userId = await _repository.GetUserIdAsync(hash);

        if (userId is null)
            return null;

        await _repository.DeleteAsync(hash);
        var newRawToken = await CreateAsync(userId);
        return (userId, newRawToken);
    }
    
    public async Task RevokeAsync(string rawToken) =>
        await _repository.DeleteAsync(RefreshToken.Hash(rawToken));
    
    public async Task RevokeAllForUserAsync(string userId) => 
        await _repository.RevokeAllForUserAsync(userId);
}
