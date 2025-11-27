using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.Utils;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using GeoAPI.CoordinateSystems;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Application.Service
{
    public class UserService(AllRepositories repository, PythonExe pythonExe)
    {
        ///<sumary>
        ///Operation for get all users from company
        ///</sumary>
        public async Task<List<UserResponseDto>> GetAllByCompany(Guid companyId)
        {
            List<UserResponseDto> users = await repository.UserRepository.GetAll(companyId);
            if (users.Count != 0)
            {
                return users;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<sumary>
        ///Operation for create user
        ///</sumary>
        public async Task Create(UserCreateDto dto, UserContext userAuthenticated)
        {
            if (await repository.UserRepository.FindAny(e =>
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
                    User user = UserMapper.FromCreateDto(dto, role.Id, userAuthenticated);
                    repository.UserRepository.Create(user);

                    var imageParts = dto.ImageBase64.Split(',');
                    var imageForDecode = imageParts.Length > 1 ? imageParts[1] : dto.ImageBase64;

                    byte[] imageBytes = Convert.FromBase64String(imageForDecode);

                    var embeddingUser = pythonExe.ExtractEmbedding(imageBytes);
                    String embeddingSerialized = JsonConvert.SerializeObject(embeddingUser);

                    UserFace userFace =
                        new UserFace(user.Id, embeddingSerialized, userAuthenticated.Id);
                    repository.UserFaceRepository.Create(userFace);

                    List<UserShift> userShifts = new List<UserShift>();
                    foreach (var shift in dto.UserShift)
                    {
                        userShifts.Add(ShiftMapper.FromCreateUserDto(shift, user.Id, userAuthenticated.Id));
                    }

                    repository.UserShiftRepository.CreateAll(userShifts);
                    await repository.Save();
                }
            }
        }

        ///<sumary>
        ///Operation for update user
        ///</sumary>
        public async Task Update(UserUpdateDto dto, UserContext userAuthenticated)
        {
            User? user = await repository.UserRepository.FindIncluding(dto.Id);
            if (user != null)
            {
                UserMapper.UpdateDataUserByUpdateDto(dto, user, userAuthenticated.Id);
                repository.UserRepository.Update(user);

                List<UserShift> userShiftsCreate = new List<UserShift>();
                List<UserShift> userShiftsUpdate = new List<UserShift>();
                foreach (var shift in dto.UserShift)
                {
                    if (shift.Id == null)
                    {
                        userShiftsCreate.Add(
                            ShiftMapper.FromCreateUserDto(shift, user.Id, userAuthenticated.Id));
                    }
                    else
                    {
                        var userShift = user.UserShifts!
                            .Find(es => es.Id == shift.Id)!;
                        ShiftMapper.UpdateUserShift(userShift, shift, userAuthenticated.Id);
                        userShiftsUpdate.Add(userShift);
                    }
                }

                List<UserShift> userShiftsRemove = new List<UserShift>();
                foreach (var shift in user.UserShifts!)
                {
                    if (!dto.UserShift.Exists(shiftDto => shiftDto.Id == shift.Id))
                    {
                        userShiftsRemove.Add(shift);
                    }
                }

                if (userShiftsRemove.Count > 0)
                {
                    user.UserShifts.RemoveAll(ex => userShiftsRemove
                        .Exists(exRemove => exRemove.Id == ex.Id));
                    repository.UserShiftRepository.DeleteAll(userShiftsRemove);
                }

                repository.UserShiftRepository.CreateAll(userShiftsCreate);
                repository.UserShiftRepository.UpdateAll(userShiftsUpdate);

                await repository.Save();
                return;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<sumary>
        ///Operation for change the status from user
        ///</sumary>
        public async Task Delete(Guid userId)
        {
            User? user = await repository.UserRepository.Find(e => e.Id == userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                repository.UserRepository.Update(user);
                await repository.Save();
                return;
            }

            throw new InvalidOperationException("Internal error, please contact the support team.");
        }

        ///<sumary>
        ///Operation for check if user can punch
        ///</sumary>
        public async Task<Boolean> CanPunchIn(CanPunchDto dto, UserContext user)
        {
            WorkShiftPunchDto? workShift =
                await repository.WorkShiftRepository.GetByUserIdAndIsActive(user.Id, user.CompanyId);
            if (workShift != null && dto.PunchType.Equals(PunchType.IN))
            {
                return true;
            }

            if (workShift != null && dto.PunchType.Equals(PunchType.OUT))
            {
                return CheckIfCanOut(workShift, dto.TimePunched, dto.PunchType);
            }

            return false;
        }

        ///<sumary>
        ///Operation for punch for user
        ///</sumary>
        public async Task<PunchInResponse> PunchIn(PunchDto dto, UserContext userContext)
        {
            User? user = await repository.UserRepository.FindForPunchIn(userContext.Id);
            if (user != null)
            {
                if (!userContext.IsAllowedBypassCoord && dto.Latitude != 0 && dto.Longitude != 0)
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
                        user!.Company.CompanyGeolocation!.Coord.Longitude,
                        user.Company.CompanyGeolocation.Coord.Latitude));

                    Double distance = userCoord.Distance(companyCoord);
                    if (distance > user.Company.CompanyGeolocation.RadiusMeters)
                    {
                        return PunchInResponse.OFR;
                    }
                }
                else if (!userContext.IsAllowedBypassCoord && dto is { Latitude: 0, Longitude: 0 })
                {
                    throw new BadHttpRequestException("Ocorreu um erro");
                }


                WorkShift? ws = user.UserShifts?.Find(es => es.IsActive)?.WorkShift;
                if (ws != null)
                {
                    byte[] base64 = Convert.FromBase64String(dto.ImageBase64.Split(',')[1]);
                    var embeddingToVerify = pythonExe.ExtractEmbedding(base64);

                    if (embeddingToVerify != null && user.UserFaces != null)
                    {
                        double? comparing = pythonExe.CompareEmbeddings(embeddingToVerify,
                            JsonConvert.DeserializeObject<List<double>>(user.UserFaces.First().FaceTemplate)!);
                        if (comparing < 50.0)
                        {
                            return PunchInResponse.FNC;
                        }

                        Boolean timeCheked = CheckTime(WorkShiftMapper.FromWorkShift(ws),
                            TimeOnly.FromDateTime(dto.TimePunched), dto.Type);

                        UserAttendance userAttendance =
                            UserAttendanceMapper.UserAttendanceFromDto(dto, userContext.Id,
                                timeCheked, timeCheked
                                    ? null
                                    : TimeOnly.FromTimeSpan(TimeOnly.FromDateTime(dto.TimePunched) -
                                                            TimeOnly.FromDateTime(DateTime.UtcNow)));
                        repository.UserAttendanceRepository.Create(userAttendance);
                        await repository.Save();

                        return PunchInResponse.SCC;
                    }
                }
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<sumary>
        ///Operation for refresh information from user
        ///</sumary>
        public async Task<UserInfoLoginDto> RefreshUserInformation(UserContext user)
        {
            TreatedUserInformationDto treatUser =
                await repository.UserRepository.FindUserInformation(user.Email!)! ??
                throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
            return AuthMapper.MapUserToUserInfoLoginDto(treatUser);
        }

        ///<sumary>
        ///Operation for punch for other user
        ///</sumary>
        public async Task PunchForUser(PunchForUserDto dto, UserContext user)
        {
            UserAttendance userAttendance = UserAttendanceMapper.UserAttendanceFromDto(dto, user.Id);
            repository.UserAttendanceRepository.Create(userAttendance);
            await repository.Save();
        }

        ///<sumary>
        ///Operation for check the time that user is punching, and return true or false, depends on time of WorkShift
        ///</sumary>
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

        ///<sumary>
        ///Operation for check if user can punch out in correctly time
        ///</sumary>
        private Boolean CheckIfCanOut(WorkShiftPunchDto ws, TimeOnly time, PunchType type)
        {
            return ws.EndTime.AddMinutes(-ws.ToleranceMinutes) <= time;
        }
    }
}
