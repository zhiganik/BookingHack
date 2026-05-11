using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace BookingHack.Core.Swagger;

public class AuthorizationDescriptionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.DeclaringType!
            .GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (attributes.Count == 0) return;

        var roles = attributes
            .Where(a => a.Roles is not null)
            .Select(a => a.Roles!)
            .ToList();

        var policies = attributes
            .Where(a => a.Policy is not null)
            .Select(a => a.Policy!)
            .ToList();

        var parts = new List<string>();
        if (roles.Count > 0) parts.Add($"Roles: `{string.Join(", ", roles)}`");
        if (policies.Count > 0) parts.Add($"Policy: `{string.Join(", ", policies)}`");

        if (parts.Count == 0) return;

        var auth = string.Join(" | ", parts);
        operation.Description = string.IsNullOrEmpty(operation.Description)
            ? $"**Auth** — {auth}"
            : $"**Auth** — {auth}\n\n{operation.Description}";
    }
}
