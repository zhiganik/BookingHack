using BookingHack.Domain.Enums;

namespace BookingHack.Application.Contracts.Requests;

public record UpdateCompanyMemberRequest(CompanyRole Role);
