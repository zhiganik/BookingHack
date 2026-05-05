using System.Text.Json.Serialization;

namespace BookingHack.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RegisterRole
{
    Customer,
    Company
}
