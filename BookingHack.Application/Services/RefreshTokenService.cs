using BookingHack.Application.Repositories;
using BookingHack.Domain.Models;

namespace BookingHack.Application.Services;

public class RefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> CreateAsync(string userId)
    {
        var (entity, rawToken) = RefreshToken.Create(userId);
        await _repository.AddAsync(entity);
        return rawToken;
    }

    public async Task<(string userId, string newRawToken)?> ValidateAndRotateAsync(string rawToken)
    {
        var stored = await _repository.GetByHashAsync(RefreshToken.Hash(rawToken));

        if (stored is null)
            return null;

        if (stored.IsRevoked)
        {
            await _repository.RevokeAllForUserAsync(stored.UserId);
            return null;
        }

        if (!stored.IsActive)
            return null;

        stored.Revoke();
        await _repository.UpdateAsync(stored);

        var newRawToken = await CreateAsync(stored.UserId);
        return (stored.UserId, newRawToken);
    }

    public async Task RevokeAsync(string rawToken)
    {
        var stored = await _repository.GetByHashAsync(RefreshToken.Hash(rawToken));
        if (stored is null || stored.IsRevoked) return;

        stored.Revoke();
        await _repository.UpdateAsync(stored);
    }
}
