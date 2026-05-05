namespace BookingHack.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Company = "Company";
    public const string Customer = "Customer";

    public static readonly IReadOnlyList<string> All = [Admin, Company, Customer];
}
