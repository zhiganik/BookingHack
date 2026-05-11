using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Contracts.Responses;

namespace BookingHack.Application.Services.Abstractions;

public interface ICompanyMemberService
{
    Task<IEnumerable<CompanyMemberResponse>?> GetAllAsync(Guid companyId);
    Task<CompanyMemberResponse?> GetAsync(Guid companyId, string userId);
    Task<CompanyMemberResponse?> AddAsync(Guid companyId, AddCompanyMemberRequest request);
    Task<CompanyMemberResponse?> UpdateAsync(Guid companyId, string userId, UpdateCompanyMemberRequest request);
    Task<bool> DeleteAsync(Guid companyId, string userId);
}
