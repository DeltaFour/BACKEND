using DeltaFour.Domain.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaFour.CrossCutting.Ioc;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddPolicies
    (
        this IServiceCollection service,
        IConfiguration configuration
    )
    {
        var superAdminPolicyName = nameof(RoleType.SUPER_ADMIN);

        service.AddAuthorizationBuilder()
            .AddPolicy(superAdminPolicyName, policy => policy.RequireClaim("Role", superAdminPolicyName));

        return service;
    }
}
