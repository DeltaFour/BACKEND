using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Infrastructure.Seeders;

public class SuperAdminSeeder(IUnitOfWork unitOfWork)
{

    private async Task<bool> SuperAdminAlreadyExists()
    {
        var email = Environment.GetEnvironmentVariable("SUPER_ADMIN_EMAIL");
        
        return await unitOfWork.EmployeeRepository.FindAny(e => e.Email == email);
    }

    private Company SaveCompanySuperAdmin()
    {
        var isActive = true;
        var cnpj = Environment.GetEnvironmentVariable("SUPER_ADMIN_COMPANY_CNPJ");
        var name = Environment.GetEnvironmentVariable("SUPER_ADMIN_COMPANY_NAME");
        var adminId = Guid.Parse(Environment.GetEnvironmentVariable("SUPER_ADMIN_ID"));
        

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

    private Role SaveRole(Guid companyId) 
    {
        var isActive = true;
        var name = nameof(RoleType.SUPER_ADMIN);

        var role = new Role()
        {
            CompanyId = companyId,
            Name = name,
            IsActive = isActive
        };

        unitOfWork.RoleRepository.Create(role);

        return role;
    }

    private void SaveEmployee(Guid companyId, Guid roleId)
    {
        var name = Environment.GetEnvironmentVariable("SUPER_ADMIN_NAME");
        var email = Environment.GetEnvironmentVariable("SUPER_ADMIN_EMAIL");
        var password = Environment.GetEnvironmentVariable("SUPER_ADMIN_PASSWORD");
        var isActive = true;
        var isConfirmed = true;
        var isAllowedBypassCoord = true;
        
        var employee = new Employee()
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

        unitOfWork.EmployeeRepository.Create(employee);
    }

    public async Task SeedAsync()
    {
        var exists = await SuperAdminAlreadyExists();

        if (exists) return;

        var company = SaveCompanySuperAdmin();
        var role = SaveRole(company.Id);

        SaveEmployee(company.Id, role.Id);

        await unitOfWork.Save();
    }
}
