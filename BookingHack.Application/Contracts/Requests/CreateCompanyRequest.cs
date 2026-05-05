namespace BookingHack.Application.Contracts.Requests;

public record CreateCompanyRequest(
    string Name,
    string TypeOfService,
    string Address,
    string? Description
);
