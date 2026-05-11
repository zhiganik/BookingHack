using BookingHack.Domain.Enums;

namespace BookingHack.Application.Contracts.Responses;

public record CompanyMemberResponse(
    string Email,
    string Name,
    CompanyRole Role,
    DateTime JoinedAt
);
