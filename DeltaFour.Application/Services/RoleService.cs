using DeltaFour.Application.Dtos.Responses.Role;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Services;

public class RoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<RoleResponse>> GetAllByCompany(Guid companyId)
    {
        var roles = await _unitOfWork.RoleRepository.GetAllByCompany(companyId);

        return roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();
    }
}
