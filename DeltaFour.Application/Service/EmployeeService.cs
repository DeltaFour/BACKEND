using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.Utils;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using GeoAPI.CoordinateSystems;
using Microsoft.AspNetCore.Http;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Security.Cryptography;
using System.Text;
using Coordinates = DeltaFour.Domain.Entities.Coordinates;

namespace DeltaFour.Application.Service
{
    public class EmployeeService(AllRepositories repository, PythonExe pythonExe)
    {
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
                    User user = EmployeeMapper.FromCreateDto(dto, role.Id, userAuthenticated);
                    repository.EmployeeRepository.Create(user);

                    var imageParts = dto.ImageBase64.Split(',');
                    var imageForDecode = imageParts.Length > 1 ? imageParts[1] : dto.ImageBase64;

                    byte[] imageBytes = Convert.FromBase64String(imageForDecode);

                    var embeddingUser = pythonExe.ExtractEmbedding(imageBytes);
                    String embeddingSerialized = JsonConvert.SerializeObject(embeddingUser);

                    UserFace userFace =
                        new UserFace(user.Id, embeddingSerialized, userAuthenticated.Id);
                    repository.EmployeeFaceRepository.Create(userFace);

                    List<UserShift> employeeShifts = new List<UserShift>();
                    foreach (var shift in dto.EmployeeShift)
                    {
                        employeeShifts.Add(ShiftMapper.FromCreateEmployeeDto(shift, user.Id, userAuthenticated.Id));
                    }

                    repository.EmployeeShiftRepository.CreateAll(employeeShifts);
                    await repository.Save();
                }
            }
        }

        public async Task Update(EmployeeUpdateDto dto, UserContext userAuthenticated)
        {
            User? employee = await repository.EmployeeRepository.FindIncluding(dto.Id);
            if (employee != null)
            {
                EmployeeMapper.UpdateDataEmployeeByUpdateDto(dto, employee, userAuthenticated.Id);
                repository.EmployeeRepository.Update(employee);

                List<UserShift> employeeShiftsCreate = new List<UserShift>();
                List<UserShift> employeeShiftsUpdate = new List<UserShift>();
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

                List<UserShift> employeeShiftsRemove = new List<UserShift>();
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
            User? employee = await repository.EmployeeRepository.Find(e => e.Id == employeeId);
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
                await repository.WorkShiftRepository.GetByEmployeeIdAndIsActive(user.Id, user.CompanyId);
            if (workShift != null && CheckTime(workShift, dto.TimePunched, dto.PunchType))
            {
                return true;
            }

            return false;
        }

        public async Task<PunchInResponse> PunchIn(PunchDto dto, UserContext user)
        {
            User? employee = await repository.EmployeeRepository.FindForPunchIn(user.Id);
            if (employee != null)
            {
                if (!user.IsAllowedBypassCoord && dto.Latitude != 0 && dto.Longitude != 0)
                {
                    var csFactory = new CoordinateSystemFactory();
                    var ctFactory = new CoordinateTransformationFactory();

                    var wgs84 = csFactory.CreateGeographicCoordinateSystem(
                        "WGS 84",
                        AngularUnit.Degrees,
                        HorizontalDatum.WGS84,
                        PrimeMeridian.Greenwich,
                        new AxisInfo("longitude", AxisOrientationEnum.East),
                        new AxisInfo("latitude", AxisOrientationEnum.North)
                    );

                    var webMercator = ProjectedCoordinateSystem.WebMercator;
                    var transformTo3857 = ctFactory.CreateFromCoordinateSystems(wgs84, webMercator);

                    var mt = transformTo3857.MathTransform;

                    var userCoord = mt.Transform(new GeoAPI.Geometries.Coordinate(dto.Longitude, dto.Latitude));
                    var companyCoord = mt.Transform(new GeoAPI.Geometries.Coordinate(
                        employee!.Company.CompanyGeolocation!.Coord.Longitude,
                        employee.Company.CompanyGeolocation.Coord.Latitude));

                    Double distance = userCoord.Distance(companyCoord);
                    if (distance > employee.Company.CompanyGeolocation.RadiusMeters)
                    {
                        return PunchInResponse.OFR;
                    }
                }
                else if (!user.IsAllowedBypassCoord && dto is { Latitude: 0, Longitude: 0 })
                {
                    throw new BadHttpRequestException("Ocorreu um erro");
                }


                WorkShift? ws = employee.EmployeeShifts?.Find(es => es.IsActive)?.WorkShift;
                if (ws != null)
                {
                    byte[] base64 = Convert.FromBase64String(dto.ImageBase64.Split(',')[1]);
                    var embeddingToVerify = pythonExe.ExtractEmbedding(base64);

                    if (embeddingToVerify != null && employee.EmployeeFaces != null)
                    {
                        double? comparing = pythonExe.CompareEmbeddings(embeddingToVerify,
                            JsonConvert.DeserializeObject<List<double>>(employee.EmployeeFaces.First().FaceTemplate)!);
                        if (comparing < 50.0)
                        {
                            return PunchInResponse.FNC;
                        }

                        Boolean timeCheked = CheckTime(WorkShiftMapper.FromWorkShift(ws),
                            TimeOnly.FromDateTime(dto.TimePunched), dto.Type);

                        UserAttendance userAttendance =
                            EmployeeAttendanceMapper.EmployeeAttendanceFromDto(dto, user.Id,
                                timeCheked, timeCheked
                                    ? null
                                    : TimeOnly.FromTimeSpan(TimeOnly.FromDateTime(dto.TimePunched) -
                                                            TimeOnly.FromDateTime(DateTime.UtcNow)));
                        repository.EmployeeAttendanceRepository.Create(userAttendance);
                        await repository.Save();

                        return PunchInResponse.SCC;
                    }
                }
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        public async Task<UserInfoForRefresh> RefreshUserInformation(UserContext user)
        {
            TreatedUserInformationDto treatUser =
                await repository.EmployeeRepository.FindUserInformation(user.Email!)! ??
                throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
            return EmployeeMapper.MapInformationForRefresh(treatUser);
        }

        public async Task PunchForUser(PunchForUserDto dto, UserContext user)
        {
            UserAttendance userAttendance = EmployeeAttendanceMapper.EmployeeAttendanceFromDto(dto, user.Id);
            repository.EmployeeAttendanceRepository.Create(userAttendance);
            await repository.Save();
        }

        private Boolean CheckTime(WorkShiftPunchDto ws, TimeOnly time, PunchType type)
        {
            if (type.Equals(PunchType.IN))
            {
                return ws.StartTime.AddMinutes(-ws.ToleranceMinutes) <= time &&
                       time <= ws.StartTime.AddMinutes(ws.ToleranceMinutes);
            }

            if (type.Equals(PunchType.OUT))
            {
                return ws.EndTime.AddMinutes(-ws.ToleranceMinutes) <= time &&
                       time <= ws.EndTime.AddMinutes(ws.ToleranceMinutes);
            }

            return true;
        }
    }
}
