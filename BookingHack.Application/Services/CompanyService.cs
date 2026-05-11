using System.Security.Claims;
using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Contracts.Responses;
using BookingHack.Application.Repositories;
using BookingHack.Application.Services.Abstractions;
using BookingHack.Domain.Constants;
using BookingHack.Domain.Enums;
using BookingHack.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingHack.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyService(ICompanyRepository repository, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<CompanyResponse>> GetAllAsync()
    {
        var companies = await _repository.GetAllAsync();
        return companies.Select(ToResponse);
    }

    public async Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        var company = await _repository.GetByIdAsync(id);
        return company is null ? null : ToResponse(company);
    }

    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request, string userId)
    {
        var company = new Company(request.Name, request.TypeOfService, request.Address, request.Description);
        company.AddMember(userId, CompanyRole.Owner);
        await _repository.AddAsync(company);

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User '{userId}' not found.");
        await _userManager.AddClaimAsync(user, new Claim(Claims.CompanyRole, nameof(CompanyRole.Owner)));

        return ToResponse(company);
    }

    public async Task<IEnumerable<CompanyResponse>> GetOwnedByUserAsync(string userId)
    {
        var companies = await _repository.GetOwnedByUserAsync(userId);
        return companies.Select(ToResponse);
    }

    public async Task<CompanyResponse?> UpdateAsync(Guid id, UpdateCompanyRequest request)
    {
        var company = await _repository.GetByIdAsync(id);
        if (company is null) return null;

        company.Update(request.Name, request.TypeOfService, request.Address, request.Description);
        await _repository.UpdateAsync(company);
        return ToResponse(company);
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _repository.DeleteAsync(id);

    private static CompanyResponse ToResponse(Company c) =>
        new(c.Id);
}
