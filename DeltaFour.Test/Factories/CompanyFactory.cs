using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class CompanyFactory(AppDbContext dbContext)
{
    private readonly Faker<Company> _faker = new Faker<Company>("pt_BR")
        .RuleFor(c => c.Id, f => Guid.NewGuid())
        .RuleFor(c => c.Name, f => f.Company.CompanyName())
        .RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##############"))
        .RuleFor(c => c.IsActive, _ => true)
        .RuleFor(c => c.CreatedBy, _ => Guid.NewGuid())
        .RuleFor(c => c.CreatedAt, f => f.Date.Past());

    public Company Generate() => _faker.Generate();

    public List<Company> Generate(int count) => _faker.Generate(count);

    public async Task<Company> CreateAsync(Action<Company>? configure = null)
    {
        var company = Generate();
        configure?.Invoke(company);

        dbContext.Companies.Add(company);
        await dbContext.SaveChangesAsync();

        return company;
    }

    public async Task<List<Company>> CreateAsync(int count, Action<Company>? configure = null)
    {
        var companies = Generate(count);

        foreach (var company in companies)
        {
            configure?.Invoke(company);
        }

        dbContext.Companies.AddRange(companies);
        await dbContext.SaveChangesAsync();

        return companies;
    }
}
