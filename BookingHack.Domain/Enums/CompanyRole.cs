using System.Text.Json.Serialization;

namespace BookingHack.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CompanyRole
{
    Owner,
    Manager
}