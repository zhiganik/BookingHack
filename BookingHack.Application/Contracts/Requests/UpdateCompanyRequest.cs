namespace BookingHack.Application.Contracts.Requests;

public record UpdateCompanyRequest(
    string Name,
    string TypeOfService,
    string Address,
    string? Description
);
