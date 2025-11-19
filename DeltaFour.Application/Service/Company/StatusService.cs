using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Service.Company;

public class StatusService
{
    private readonly IUnitOfWork _unitOfWork;

    public StatusService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Active(Guid companyId)
    {
        var company = await _unitOfWork.CompanyRepository.Find(c => c.Id == companyId);

        company!.IsActive = true;

        await _unitOfWork.Save();
    }

    public async Task Desactive(Guid companyId)
    {
        var company = await _unitOfWork.CompanyRepository.Find(c => c.Id == companyId);

        company!.IsActive = false;

        await _unitOfWork.Save();
    }
}
