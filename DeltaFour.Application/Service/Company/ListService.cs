using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Service.Company;

public class ListService(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    ///<summary>
    ///Operation for list all companies
    ///</summary>
    public async Task<ListCompaniesResponse> Get()
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
}
