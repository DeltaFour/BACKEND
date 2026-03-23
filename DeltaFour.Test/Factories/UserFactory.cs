using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Context;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Test.Factories;

public class UserFactory(AppDbContext dbContext)
{
    private readonly Faker<User> _faker = new Faker<User>("pt_BR")
        .RuleFor(u => u.Id, _ => Guid.NewGuid())
        .RuleFor(u => u.Name, f => f.Person.FullName)
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, _ => HashPassword("Test@123"))
        .RuleFor(u => u.Cellphone, f => f.Phone.PhoneNumber("###########"))
        .RuleFor(u => u.IsActive, _ => true)
        .RuleFor(u => u.IsConfirmed, _ => true)
        .RuleFor(u => u.IsAllowedBypassCoord, _ => false)
        .RuleFor(u => u.CreatedBy, _ => Guid.NewGuid())
        .RuleFor(u => u.CreatedAt, f => f.Date.Past());

    public User Generate(Guid? companyId = null, Guid? roleId = null)
    {
        var user = _faker.Generate();
        user.CompanyId = companyId ?? Guid.NewGuid();
        user.RoleId = roleId;
        return user;
    }

    public List<User> Generate(int count, Guid? companyId = null, Guid? roleId = null)
    {
        var users = _faker.Generate(count);
        foreach (var user in users)
        {
            user.CompanyId = companyId ?? Guid.NewGuid();
            user.RoleId = roleId;
        }
        return users;
    }

    public async Task<User> CreateAsync(Guid companyId, Guid? roleId = null, Action<User>? configure = null)
    {
        var user = Generate(companyId, roleId);
        configure?.Invoke(user);

        dbContext.Employees.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> CreateAsync(int count, Guid companyId, Guid? roleId = null, Action<User>? configure = null)
    {
        var users = Generate(count, companyId, roleId);

        foreach (var user in users)
        {
            configure?.Invoke(user);
        }

        dbContext.Employees.AddRange(users);
        await dbContext.SaveChangesAsync();

        return users;
    }

    public async Task<User> CreateWithPasswordAsync(Guid companyId, Guid? roleId, string plainPassword, Action<User>? configure = null)
    {
        var user = Generate(companyId, roleId);
        user.Password = HashPassword(plainPassword);
        configure?.Invoke(user);

        dbContext.Employees.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static string HashPassword(string password)
    {
        using var hash = SHA256.Create();
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashPassword = new StringBuilder();
        foreach (byte b in bytes)
        {
            hashPassword.Append(b.ToString("x2"));
        }
        return hashPassword.ToString();
    }
}
