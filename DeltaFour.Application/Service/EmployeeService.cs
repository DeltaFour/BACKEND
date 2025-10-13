using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace DeltaFour.Application.Service
{
    public class EmployeeService(AllRepositories repository)
    {
        public async Task create(EmployeeCreateDto dto, List<IFormFile> files, Employee userAuthenticated)
        {
            if (repository.EmployeeRepository.Find(e => e.Email == userAuthenticated.Email) == null)
            {
                Role? role = await repository.RoleRepository.Find(r => r.Name == dto.RoleName);
                if (role != null)
                {
                    Employee employee = EmployeeMapper.fromCreateDto(dto, role.Id, userAuthenticated);
                    repository.EmployeeRepository.Create(employee);
                    if (files.Count > 0)
                    {
                        foreach (var formFile in files)
                        {
                            String fileName = Path.GetFileName(
                                $"{userAuthenticated.CompanyId}_{Guid.NewGuid()}_{dto.RoleName}");

                        }
                    }
                }
            }
        }
    }
}
