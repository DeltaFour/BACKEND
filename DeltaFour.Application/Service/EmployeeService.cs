using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace DeltaFour.Application.Service
{
    public class EmployeeService(AllRepositories repository)
    {
        static EmployeeService()
        {
            IMAGE_FOLDER = "Image";
        }
        private static readonly String IMAGE_FOLDER;

        public async Task<List<EmployeeResponseDto>> GetAllByCompany(Guid companyId)
        {
            List<Employee> employees = await repository.EmployeeRepository.GetAll(companyId);
            if (employees.Count != 0)
            {
                List<EmployeeResponseDto> list = new List<EmployeeResponseDto>();
                foreach (Employee employee in employees)
                {
                    list.Add(EmployeeMapper.fromListEmployee(employee));
                }

                return list;
            }
            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }
        
        public async Task Create(EmployeeCreateDto dto, List<IFormFile> files, Employee userAuthenticated)
        {
            if (await repository.EmployeeRepository.Find(e =>
                    e.Email == userAuthenticated.Email && e.CompanyId == userAuthenticated.CompanyId) == null)
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
                            String filePath = Path.Combine(IMAGE_FOLDER, fileName);
                            try
                            {
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new BadHttpRequestException(ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
