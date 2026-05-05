using BookingHack.Domain.Enums;

namespace BookingHack.Application.Contracts.Requests;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    RegisterRole Role
);