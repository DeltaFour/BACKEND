using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Infrastructure.Seeders;

public class SuperAdminSeeder(IUnitOfWork unitOfWork)
{
    private static readonly Guid IdCompanyId = Guid.Parse(Environment.GetEnvironmentVariable("SUPER_ADMIN_ID"));
    private static readonly Guid RoleAdminId = Guid.Parse(Environment.GetEnvironmentVariable("ROLE_SUPER_ADMIN_ID"));

    private async Task<bool> SuperAdminAlreadyExists()
    {
        var email = Environment.GetEnvironmentVariable("SUPER_ADMIN_EMAIL");

        return await unitOfWork.UserRepository.FindAny(e => e.Email == email);
    }

    private Company SaveCompanySuperAdmin()
    {
        var isActive = true;
        var cnpj = Environment.GetEnvironmentVariable("SUPER_ADMIN_COMPANY_CNPJ");
        var name = Environment.GetEnvironmentVariable("SUPER_ADMIN_COMPANY_NAME");
        var adminId = IdCompanyId;


        var company = new Company()
        {
            Id = adminId,
            IsActive = isActive,
            Cnpj = cnpj,
            Name = name,
        };

        unitOfWork.CompanyRepository.Create(company);

        return company;
    }

    private Role SaveRole()
    {
        var isActive = true;
        var name = nameof(RoleType.SUPER_ADMIN);

        var role = new Role()
        {
            Id = RoleAdminId,
            CompanyId = IdCompanyId,
            Name = name,
            IsActive = isActive
        };

        unitOfWork.RoleRepository.Create(role);

        return role;
    }

    private void SaveEmployee(Guid companyId, Guid roleId)
    {
        using var hash = SHA256.Create();
        byte[] bytes =
            hash.ComputeHash(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SUPER_ADMIN_PASSWORD")));
        var hashPassowrd = new StringBuilder();
        foreach (byte b in bytes)
        {
            hashPassowrd.Append(b.ToString("x2"));
        }

        var name = Environment.GetEnvironmentVariable("SUPER_ADMIN_NAME");
        var email = Environment.GetEnvironmentVariable("SUPER_ADMIN_EMAIL");
        var password = hashPassowrd.ToString();
        var isActive = true;
        var isConfirmed = true;
        var isAllowedBypassCoord = true;

        var employee = new User()
        {
            CompanyId = companyId,
            RoleId = roleId,
            Name = name,
            Email = email,
            Password = password,
            IsActive = isActive,
            IsConfirmed = isConfirmed,
            IsAllowedBypassCoord = isAllowedBypassCoord,
        };

        unitOfWork.UserRepository.Create(employee);
    }

    public async Task SeedAsync()
    {
        var exists = await SuperAdminAlreadyExists();

        if (exists) return;

        SaveCompanySuperAdmin();
        SaveRole();

        SaveEmployee(IdCompanyId, RoleAdminId);

        await unitOfWork.Save();
    }
}
