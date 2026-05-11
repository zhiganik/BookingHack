using BookingHack.Domain.Enums;

namespace BookingHack.Application.Contracts.Requests;

public record AddCompanyMemberRequest(string UserId, CompanyRole Role);
