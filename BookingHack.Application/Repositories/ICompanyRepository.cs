using BookingHack.Domain.Models;

namespace BookingHack.Application.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(Guid id);
    Task<Company> AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(Guid id);
    Task<Company?> GetByIdWithMembersAsync(Guid id);
    Task<IEnumerable<Company>> GetOwnedByUserAsync(string userId);
}
