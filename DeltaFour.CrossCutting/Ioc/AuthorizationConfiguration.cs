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
        var rhPolicy = nameof(RoleType.RH);
        var employeePolicy = nameof(RoleType.EMPLOYEE);

        service.AddAuthorizationBuilder()
            .AddPolicy(superAdminPolicyName, policy => policy.RequireClaim("Role", superAdminPolicyName))
            .AddPolicy(rhPolicy, policy => policy.RequireClaim("Role", rhPolicy))
            .AddPolicy(employeePolicy, policy => policy.RequireClaim("Role", employeePolicy))
            .AddPolicy("RH_OR_EMPLOYEE",
                policy => policy.RequireAssertion(context =>
                    context.User.HasClaim("Role", rhPolicy) || context.User.HasClaim("Role", employeePolicy)));

        return service;
    }
}
