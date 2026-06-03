using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Department;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Services;

public class DepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Create(CreateDepartmentRequest request, Guid userId, Guid companyId)
    {
        var department = new Department
        {
            Name = request.Name,
            CompanyId = companyId,
            CreatedBy = userId,
        };

        _unitOfWork.DepartmentRepository.Create(department);

        await _unitOfWork.Save();
    }

    public async Task<ListDepartmentsResponse> List(Guid companyId)
    {
        var departments = await _unitOfWork.DepartmentRepository.FindAll(
            d => d.CompanyId == companyId,
            d => new GetDepartmentItemResponse
            {
                Id = d.Id,
                Name = d.Name,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
            });

        return new ListDepartmentsResponse
        {
            Departments = departments,
        };
    }

    public async Task Update(Guid id, UpdateDepartmentRequest request, Guid userId, Guid companyId)
    {
        var department = await _unitOfWork.DepartmentRepository.Find(d => d.Id == id && d.CompanyId == companyId);

        if (department == null)
        {
            throw new Exception("Departamento não encontrado");
        }

        department.Name = request.Name;
        department.UpdatedAt = DateTime.UtcNow;
        department.UpdatedBy = userId;

        _unitOfWork.DepartmentRepository.Update(department);

        await _unitOfWork.Save();
    }

    public async Task Delete(Guid id, Guid companyId)
    {
        var department = await _unitOfWork.DepartmentRepository.Find(d => d.Id == id && d.CompanyId == companyId);

        if (department == null)
        {
            throw new Exception("Departamento não encontrado");
        }

        var hasUsers = await _unitOfWork.DepartmentRepository.HasUsers(id);

        if (hasUsers)
        {
            throw new Exception("Não é possível remover departamento vinculado a usuários");
        }

        _unitOfWork.DepartmentRepository.Delete(department);

        await _unitOfWork.Save();
    }
}
