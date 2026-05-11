using System.Security.Claims;
using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Contracts.Responses;
using BookingHack.Application.Repositories;
using BookingHack.Application.Services.Abstractions;
using BookingHack.Domain.Constants;
using BookingHack.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingHack.Application.Services;

public class CompanyMemberService : ICompanyMemberService
{
    private readonly ICompanyRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyMemberService(ICompanyRepository repository, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<CompanyMemberResponse>?> GetAllAsync(Guid companyId)
    {
        var company = await _repository.GetByIdWithMembersAsync(companyId);
        return company?.Members.Select(ToResponse);
    }

    public async Task<CompanyMemberResponse?> GetAsync(Guid companyId, string userId)
    {
        var company = await _repository.GetByIdWithMembersAsync(companyId);
        var member = company?.Members.FirstOrDefault(m => m.UserId == userId);
        return member is null ? null : ToResponse(member);
    }

    public async Task<CompanyMemberResponse?> AddAsync(Guid companyId, AddCompanyMemberRequest request)
    {
        var company = await _repository.GetByIdWithMembersAsync(companyId);
        if (company is null) return null;

        company.AddMember(request.UserId, request.Role);
        await _repository.UpdateAsync(company);

        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User '{request.UserId}' not found.");
        await _userManager.AddClaimAsync(user, new Claim(Claims.CompanyRole, request.Role.ToString()));

        return new CompanyMemberResponse(
            user.Email!,
            $"{user.FirstName} {user.LastName}",
            request.Role,
            DateTime.UtcNow);
    }

    public async Task<CompanyMemberResponse?> UpdateAsync(Guid companyId, string userId, UpdateCompanyMemberRequest request)
    {
        var company = await _repository.GetByIdWithMembersAsync(companyId);
        if (company is null) return null;

        var member = company.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) return null;

        var oldRole = member.Role;
        company.UpdateMemberRole(userId, request.Role);
        await _repository.UpdateAsync(company);

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User '{userId}' not found.");
        await _userManager.RemoveClaimAsync(user, new Claim(Claims.CompanyRole, oldRole.ToString()));
        await _userManager.AddClaimAsync(user, new Claim(Claims.CompanyRole, request.Role.ToString()));

        return ToResponse(member);
    }

    public async Task<bool> DeleteAsync(Guid companyId, string userId)
    {
        var company = await _repository.GetByIdWithMembersAsync(companyId);
        if (company is null) return false;

        var member = company.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) return false;

        var role = member.Role;
        company.RemoveMember(userId);
        await _repository.UpdateAsync(company);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
            await _userManager.RemoveClaimAsync(user, new Claim(Claims.CompanyRole, role.ToString()));

        return true;
    }

    private static CompanyMemberResponse ToResponse(CompanyMember m) =>
        new(m.User.Email!, $"{m.User.FirstName} {m.User.LastName}", m.Role, m.JoinedAt);
}
