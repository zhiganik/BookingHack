using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Contracts.Responses;

namespace BookingHack.Application.Services.Abstractions;

public interface ICompanyService
{
    Task<IEnumerable<CompanyResponse>> GetAllAsync();
    Task<CompanyResponse?> GetByIdAsync(Guid id);
    Task<CompanyResponse> CreateAsync(CreateCompanyRequest request);
    Task<CompanyResponse?> UpdateAsync(Guid id, UpdateCompanyRequest request);
    Task<bool> DeleteAsync(Guid id);
}
