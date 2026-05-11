using BookingHack.Application.Repositories;
using BookingHack.Domain.Enums;
using BookingHack.Domain.Models;
using BookingHack.Infrastructure.PostgreSql.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BookingHack.Infrastructure.PostgreSql.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly BookingHackDbContext _context;

    public CompanyRepository(BookingHackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllAsync() =>
        await _context.Companies.ToListAsync();

    public async Task<Company?> GetByIdAsync(Guid id) =>
        await _context.Companies.FindAsync(id);

    public async Task<Company> AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return false;

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Company?> GetByIdWithMembersAsync(Guid id) =>
        await _context.Companies
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Company>> GetOwnedByUserAsync(string userId) =>
        await _context.Companies
            .Where(c => c.Members.Any(m => m.UserId == userId && m.Role == CompanyRole.Owner))
            .ToListAsync();
}
