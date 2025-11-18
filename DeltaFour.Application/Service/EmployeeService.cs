using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.Utils;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Coordinates = DeltaFour.Domain.Entities.Coordinates;

namespace DeltaFour.Application.Service
{
    public class EmployeeService(AllRepositories repository, PythonExe pythonExe)
    {
        static EmployeeService()
        {
            ImageFolder = "../Image";
        }

        private static readonly String ImageFolder;

        public async Task<List<EmployeeResponseDto>> GetAllByCompany(Guid companyId)
        {
            List<EmployeeResponseDto> employees = await repository.EmployeeRepository.GetAll(companyId);
            if (employees.Count != 0)
            {
                return employees;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        public async Task Create(EmployeeCreateDto dto, UserContext userAuthenticated)
        {
            if (await repository.EmployeeRepository.FindAny(e =>
                    e.Email == dto.Email && e.CompanyId == userAuthenticated.CompanyId) is false)
            {
                Role? role = await repository.RoleRepository.Find(r => r.Name == dto.RoleName);
                if (role != null)
                {
                    using var hash = SHA256.Create();
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(dto.Password!));
                    var hashPassowrd = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        hashPassowrd.Append(b.ToString("x2"));
                    }

                    dto.Password = hashPassowrd.ToString();
                    Employee employee = EmployeeMapper.FromCreateDto(dto, role.Id, userAuthenticated);
                    repository.EmployeeRepository.Create(employee);

                    String imageType = dto.ImageBase64.Split(';')[0].Split('/')[1];
                    String imageForDecode = dto.ImageBase64.Split(',')[1];

                    String fileName = Path.GetFileName(
                        $"{userAuthenticated.CompanyId}_{employee.Id}_{dto.Name}.{imageType}");
                    String filePath = Path.Combine(ImageFolder, fileName);

                    byte[] imageBytes = Convert.FromBase64String(imageForDecode);

                    var embeddingUser = pythonExe.ExtractEmbedding(imageBytes);
                    String embeddingSerialized = JsonConvert.SerializeObject(embeddingUser);

                    EmployeeFace employeeFace =
                        new EmployeeFace(employee.Id, embeddingSerialized, userAuthenticated.Id);
                    repository.EmployeeFaceRepository.Create(employeeFace);

                    await File.WriteAllBytesAsync(filePath, imageBytes);

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

        public async Task Update(EmployeeUpdateDto dto, UserContext userAuthenticated)
        {
            Employee? employee = await repository.EmployeeRepository.FindIncluding(dto.Id);
            if (employee != null)
            {
                EmployeeMapper.UpdateDataEmployeeByUpdateDto(dto, employee, userAuthenticated.Id);
                repository.EmployeeRepository.Update(employee);

                List<EmployeeShift> employeeShiftsCreate = new List<EmployeeShift>();
                List<EmployeeShift> employeeShiftsUpdate = new List<EmployeeShift>();
                foreach (var shift in dto.EmployeeShift)
                {
                    if (shift.Id == null)
                    {
                        employeeShiftsCreate.Add(
                            ShiftMapper.FromCreateEmployeeDto(shift, employee.Id, userAuthenticated.Id));
                    }
                    else
                    {
                        var employeeShift = employee.EmployeeShifts!
                            .Find(es => es.Id == shift.Id)!;
                        ShiftMapper.UpdateEmployeeShift(employeeShift, shift, userAuthenticated.Id);
                        employeeShiftsUpdate.Add(employeeShift);
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

                if (employeeShiftsRemove.Count > 0)
                {
                    employee.EmployeeShifts.RemoveAll(ex => employeeShiftsRemove
                        .Exists(exRemove => exRemove.Id == ex.Id));
                    repository.EmployeeShiftRepository.DeleteAll(employeeShiftsRemove);
                }

                repository.EmployeeShiftRepository.CreateAll(employeeShiftsCreate);
                repository.EmployeeShiftRepository.UpdateAll(employeeShiftsUpdate);

                await repository.Save();
                return;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        public async Task Delete(Guid employeeId)
        {
            Employee? employee = await repository.EmployeeRepository.Find(e => e.Id == employeeId);
            if (employee != null)
            {
                employee.IsActive = !employee.IsActive;
                repository.EmployeeRepository.Update(employee);
                await repository.Save();
                return;
            }

            throw new InvalidOperationException("Internal error, please contact the support team.");
        }

        public async Task<Boolean> CanPunchIn(CanPunchDto dto, UserContext user)
        {
            WorkShiftPunchDto? workShift =
                await repository.WorkShiftRepository.GetByTimeAndEmployeeId(dto.TimePunched, user.Id, user.CompanyId);
            if (workShift != null && (dto.PunchType == PunchType.IN &&
                    dto.TimePunched.IsBetween(
                        workShift.StartTime.AddMinutes(-workShift.ToleranceMinutes),
                        workShift.StartTime.AddMinutes(workShift.ToleranceMinutes)) || dto.PunchType == PunchType.OUT &&
                    dto.TimePunched.IsBetween(
                        workShift.EndTime.AddMinutes(-workShift.ToleranceMinutes),
                        workShift.EndTime.AddMinutes(workShift.ToleranceMinutes))))
            {
                return true;
            }

            return false;
        }

        public async Task<PunchInResponse> PunchIn(PunchDto dto, UserContext user)
        {
            Employee? employee = await repository.EmployeeRepository.FindForPunchIn(user.Id);
            if (employee != null)
            {
                if (!user.IsAllowedBypassCoord && dto is { latitude: > 0, longitude: > 0 })
                {
                    var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 3857);

                    var userPoint = geoFactory.CreatePoint(new Coordinate(dto.longitude, dto.latitude));
                    var companyPoint = geoFactory.CreatePoint(new Coordinate(
                        employee!.Company.CompanyGeolocation!.Coord.Longitude,
                        employee.Company.CompanyGeolocation.Coord.Latitude));

                    Double distance = userPoint.Distance(companyPoint);
                    if (distance > employee.Company.CompanyGeolocation.RadiusMeters)
                    {
                        return PunchInResponse.OFR;
                    }
                }
                else if (!user.IsAllowedBypassCoord && dto is { latitude: 0, longitude: 0 })
                {
                    throw new BadHttpRequestException("Ocorreu um erro");
                }


                byte[] base64 = Convert.FromBase64String(dto.ImageBase64.Split(',')[1]);
                var embeddingToVerify = pythonExe.ExtractEmbedding(base64);
                if (embeddingToVerify != null && employee.EmployeeFaces != null)
                {
                    double? comparing = pythonExe.CompareEmbeddings(embeddingToVerify,
                        JsonConvert.DeserializeObject<List<double>>(employee.EmployeeFaces.First().FaceTemplate)!);
                    if (comparing < 75.0)
                    {
                        return PunchInResponse.FNC;
                    }

                    EmployeeAttendance employeeAttendance = new EmployeeAttendance()
                    {
                        EmployeeId = user.Id,
                        PunchTime = dto.TimePunched,
                        PunchType = dto.Type,
                        ShiftType = dto.ShiftType,
                        Coord = new Coordinates(dto.longitude, dto.latitude),
                        CreatedBy = user.Id,
                    };
                    repository.EmployeeAttendanceRepository.Create(employeeAttendance);
                    await repository.Save();

                    return PunchInResponse.SCC;
                }
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }
    }
}
