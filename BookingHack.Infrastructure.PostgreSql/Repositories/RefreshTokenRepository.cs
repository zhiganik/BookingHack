using BookingHack.Application.Repositories;
using BookingHack.Domain.Models;
using BookingHack.Infrastructure.PostgreSql.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BookingHack.Infrastructure.PostgreSql.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BookingHackDbContext _context;

    public RefreshTokenRepository(BookingHackDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash) =>
        await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.Revoke();

        await _context.SaveChangesAsync();
    }
}
