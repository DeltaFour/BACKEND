using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Domain.Entities;
using CompanyEntity = DeltaFour.Domain.Entities.Company;
using DeltaFour.Domain.IRepositories;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Application.Service.Company;

public class CreateService(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private CompanyEntity SaveCompany(CreateCompanyRequest request, Guid userId)
    {
        var company = new CompanyEntity()
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            IsActive = true,
            CreatedBy = userId,
        };

        _unitOfWork.CompanyRepository.Create(company);

        return company;
    }

    private Role SaveRole(Guid companyId, Guid userId)
    {
        var role = new Role()
        {
            Name = "ADMIN",
            CompanyId = companyId,
            IsActive = true,
            CreatedBy = userId,
        };

        _unitOfWork.RoleRepository.Create(role);

        return role;
    }

    private User SaveFirstEmployee(
        EmployeeRequest request,
        Guid companyId,
        Guid createdBy,
        Guid roleId
    )
    {
        using var hash = SHA256.Create();
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        var hashPassowrd = new StringBuilder();
        foreach (byte b in bytes)
        {
            hashPassowrd.Append(b.ToString("x2"));
        }
        var employee = new User()
        {
            Name = request.Name,
            Email = request.Email,
            Password = hashPassowrd.ToString(),
            CreatedBy = createdBy,
            CompanyId = companyId,
            RoleId = roleId,
            IsActive = true,
            IsConfirmed = true,
        };

        _unitOfWork.EmployeeRepository.Create(employee);

        return employee;
    }

    public async Task Create(CreateCompanyRequest request, Guid createdBy)
    {
        var company = SaveCompany(request, createdBy);
        var role = SaveRole(company.Id, createdBy);
        
        SaveFirstEmployee(request.Employee!, company.Id, createdBy, role.Id);

        await _unitOfWork.Save();
    }
}
