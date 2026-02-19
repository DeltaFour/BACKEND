using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Service.Company;

public class StatusService
{
    private readonly IUnitOfWork _unitOfWork;

    public StatusService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    ///<summary>
    ///Operation for change the status from specific company
    ///</summary>
    public async Task ChangeStatus(Guid companyId)
    {
        var company = await _unitOfWork.CompanyRepository.Find(c => c.Id == companyId);

        company!.IsActive = !company.IsActive;

        await _unitOfWork.Save();
    }
}
