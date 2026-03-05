using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Application.Services;

public class CompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Create(CreateCompanyRequest request, Guid userId)
    {
        var company = new Company()
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            IsActive = true,
            CreatedBy = userId,
        };


        var role = new Role()
        {
            Name = "ADMIN",
            CompanyId = company.Id,
            IsActive = true,
            CreatedBy = userId,
        };


        using var hash = SHA256.Create();
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(request.User!.Password));
        var hashPassowrd = new StringBuilder();
        foreach (byte b in bytes)
        {
            hashPassowrd.Append(b.ToString("x2"));
        }
        var user = new User()
        {
            Name = request.Name,
            Email = request.User.Email,
            Password = hashPassowrd.ToString(),
            CreatedBy = userId,
            CompanyId = company.Id,
            RoleId = role.Id,
            IsActive = true,
            IsConfirmed = true,
        };

        _unitOfWork.CompanyRepository.Create(company);
        _unitOfWork.RoleRepository.Create(role);
        _unitOfWork.UserRepository.Create(user);

        await _unitOfWork.Save();
    }

    public async Task<ListCompaniesResponse> List()
    {
        var companies = await _unitOfWork.CompanyRepository.FindAll(c => new GetCompaniesItemResponse
        {
            Id = c.Id,
            Name = c.Name,
            Cnpj = c.Cnpj,
            IsActive = c.IsActive,
        });

        return new ListCompaniesResponse
        {
            Data = companies,
        };
    }

    public async Task ChangeStatus(Guid companyId)
    {
        var company = await _unitOfWork.CompanyRepository.Find(c => c.Id == companyId);

        company!.IsActive = !company.IsActive;

        await _unitOfWork.Save();
    }
}
