using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class RoleFactory(AppDbContext dbContext)
{
    private readonly Faker<Role> _faker = new Faker<Role>("pt_BR")
        .RuleFor(r => r.Id, _ => Guid.NewGuid())
        .RuleFor(r => r.Name, f => f.PickRandom<RoleType>().ToString())
        .RuleFor(r => r.IsActive, _ => true)
        .RuleFor(r => r.CreatedBy, _ => Guid.NewGuid())
        .RuleFor(r => r.CreatedAt, f => f.Date.Past());

    public Role Generate(Guid? companyId = null)
    {
        var role = _faker.Generate();
        role.CompanyId = companyId ?? Guid.NewGuid();
        return role;
    }

    public List<Role> Generate(int count, Guid? companyId = null)
    {
        var roles = _faker.Generate(count);
        foreach (var role in roles)
        {
            role.CompanyId = companyId ?? Guid.NewGuid();
        }
        return roles;
    }

    public async Task<Role> CreateAsync(Guid companyId, Action<Role>? configure = null)
    {
        var role = Generate(companyId);
        configure?.Invoke(role);

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return role;
    }

    public async Task<List<Role>> CreateAsync(int count, Guid companyId, Action<Role>? configure = null)
    {
        var roles = Generate(count, companyId);

        foreach (var role in roles)
        {
            configure?.Invoke(role);
        }

        dbContext.Roles.AddRange(roles);
        await dbContext.SaveChangesAsync();

        return roles;
    }
}
