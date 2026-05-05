namespace BookingHack.Application.Contracts.Responses;

public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiry,
    string RefreshToken
);
