using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Net;
using Coordinates = DeltaFour.Domain.Entities.Coordinates;

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
            List<EmployeeResponseDto> employees = await repository.EmployeeRepository.GetAll(companyId);
            if (employees.Count != 0)
            {
                return employees;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        public async Task Create(EmployeeCreateDto dto, List<IFormFile> files, Employee userAuthenticated)
        {
            if (await repository.EmployeeRepository.FindAny(e =>
                    e.Email == userAuthenticated.Email && e.CompanyId == userAuthenticated.CompanyId))
            {
                Role? role = await repository.RoleRepository.Find(r => r.Name == dto.RoleName);
                if (role != null)
                {
                    Employee employee = EmployeeMapper.FromCreateDto(dto, role.Id, userAuthenticated);
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

                    List<EmployeeShift> employeeShifts = new List<EmployeeShift>();
                    foreach (var shift in dto.EmployeeShift)
                    {
                        employeeShifts.Add(ShiftMapper.FromCreateEmployeeDto(shift, employee.Id, userAuthenticated.Id));
                    }

                    repository.EmployeeShiftRepository.CreateAll(employeeShifts);
                    await repository.Save();
                }
            }
        }

        public async Task Update(EmployeeUpdateDto dto, Employee userAuthenticated)
        {
            Employee? employee = await repository.EmployeeRepository.FindIncluding(dto.Id);
            if (employee != null)
            {
                EmployeeMapper.UpdateDataEmployeeByUpdateDto(dto, employee, userAuthenticated.Id);
                repository.EmployeeRepository.Update(employee);

                List<EmployeeShift> employeeShiftsCreate = new List<EmployeeShift>();
                foreach (var shift in dto.EmployeeShift)
                {
                    if (shift.Id == null)
                    {
                        employeeShiftsCreate.Add(
                            ShiftMapper.FromCreateEmployeeDto(shift, employee.Id, userAuthenticated.Id));
                    }
                    else
                    {
                        ShiftMapper.UpdateEmployeeShift(employee.EmployeeShifts!
                            .Find(es => es.Id == shift.Id)!, shift, userAuthenticated.Id);
                    }
                }

                List<EmployeeShift> employeeShiftsRemove = new List<EmployeeShift>();
                foreach (var shift in employee.EmployeeShifts!)
                {
                    if (!dto.EmployeeShift.Exists(shiftDto => shiftDto.Id == shift.Id))
                    {
                        employeeShiftsRemove.Add(shift);
                    }
                }

                employee.EmployeeShifts.RemoveAll(ex => employeeShiftsRemove
                    .Exists(exRemove => exRemove.Id == ex.Id));

                repository.EmployeeShiftRepository.CreateAll(employeeShiftsCreate);
                repository.EmployeeShiftRepository.UpdateAll(employee.EmployeeShifts);
                repository.EmployeeShiftRepository.DeleteAll(employeeShiftsRemove);

                await repository.Save();
            }
        }

        public async Task Delete(Guid employeeId)
        {
            Employee? employee = await repository.EmployeeRepository.Find(e => e.Id == employeeId);
            if (employee != null)
            {
                employee.IsActive = false;
                repository.EmployeeRepository.Update(employee);
                await repository.Save();
            }
        }

        public async Task<Boolean> PunchIn(PunchDto dto, Employee user)
        {
            if (!user.IsAllowedBypassCoord && dto.latitude > 0 && dto.longitude > 0)
            {
                var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 3857);

                var userPoint = geoFactory.CreatePoint(new Coordinate(dto.longitude, dto.latitude));
                var companyPoint = geoFactory.CreatePoint(new Coordinate(
                    user.Company.CompanyGeolocation!.Coord.Longitude,
                    user.Company.CompanyGeolocation!.Coord.Latitude));

                Double distance = userPoint.Distance(companyPoint);
                if (distance > user.Company.CompanyGeolocation.RadiusMeters)
                {
                    return false;
                }
            }
            
            EmployeeAttendance employeeAttendance = new EmployeeAttendance()
            {
                EmployeeId = user.Id,
                PunchTime = DateTime.Now,
                PunchType = dto.Type,
                Coord = new Coordinates(dto.longitude, dto.latitude),
                CreatedBy = user.Id,
            };
            repository.EmployeeAttendanceRepository.Create(employeeAttendance);
            await repository.Save();
            
            return true;
        }
    }
}
