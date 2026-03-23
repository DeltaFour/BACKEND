using DeltaFour.Application.Dtos;
using DeltaFour.Application.Integrations;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Domain.ValueObjects.Dtos;
using GeoAPI.CoordinateSystems;
using Microsoft.AspNetCore.Http;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Application.Services
{
    public class UserService(IUnitOfWork unitOfWork, IFaceRecognitionIntegration faceRecognitionIntegration)
    {
        ///<summary>
        ///Operation for get all users from company
        ///</summary>
        public async Task<List<UserResponseDto>> GetAllByCompany(Guid companyId)
        {
            var users = await unitOfWork.UserRepository.GetAll(companyId);

            if (users.Count != 0)
            {
                return users;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<summary>
        ///Operation for create user
        ///</summary>
        public async Task Create(UserCreateDto dto, UserContext userAuthenticated)
        {
            if (await unitOfWork.UserRepository.FindAny(e =>
                    e.Email == dto.Email && e.CompanyId == userAuthenticated.CompanyId) is false)
            {
                Role? role = await unitOfWork.RoleRepository.Find(r => r.Name == dto.RoleName);
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

                    var user = UserMapper.FromCreateDto(dto, role.Id, userAuthenticated);

                    unitOfWork.UserRepository.Create(user);

                    var embedding = await faceRecognitionIntegration.GetFaceEmbeddings(dto.ImageBase64);
                    var userFace = new UserFace(user.Id, embedding, userAuthenticated.Id);

                    unitOfWork.UserFaceRepository.Create(userFace);

                    var userShifts = new List<UserShift>();

                    foreach (var shift in dto.UserShift)
                    {
                        userShifts.Add(ShiftMapper.FromCreateUserDto(shift, user.Id, userAuthenticated.Id));
                    }

                    unitOfWork.UserShiftRepository.CreateAll(userShifts);

                    await unitOfWork.Save();
                }
            }
        }

        ///<summary>
        ///Operation for update user
        ///</summary>
        public async Task Update(UserUpdateDto dto, UserContext userAuthenticated)
        {
            var user = await unitOfWork.UserRepository.FindIncluding(dto.Id);

            if (user != null)
            {
                UserMapper.UpdateDataUserByUpdateDto(dto, user, userAuthenticated.Id);
                unitOfWork.UserRepository.Update(user);

                var userShiftsCreate = new List<UserShift>();
                var userShiftsUpdate = new List<UserShift>();

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

                var userShiftsRemove = new List<UserShift>();

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
                    unitOfWork.UserShiftRepository.DeleteAll(userShiftsRemove);
                }

                unitOfWork.UserShiftRepository.CreateAll(userShiftsCreate);
                unitOfWork.UserShiftRepository.UpdateAll(userShiftsUpdate);

                await unitOfWork.Save();
                return;
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<summary>
        ///Operation for change the status from user
        ///</summary>
        public async Task Delete(Guid userId)
        {
            var user = await unitOfWork.UserRepository.Find(e => e.Id == userId);

            if (user != null)
            {
                user.IsActive = !user.IsActive;
                unitOfWork.UserRepository.Update(user);
                await unitOfWork.Save();
                return;
            }

            throw new InvalidOperationException("Internal error, please contact the support team.");
        }

        ///<summary>
        ///Operation for check if user can punch
        ///</summary>
        public async Task<Boolean> CanPunchIn(CanPunchDto dto, UserContext user)
        {
            var workShift = await unitOfWork.WorkShiftRepository.GetByUserIdAndIsActive(user.Id, user.CompanyId);

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

        ///<summary>
        ///Operation for punch for user
        ///</summary>
        public async Task<PunchInResponse> PunchIn(PunchDto dto, UserContext userContext)
        {
            var user = await unitOfWork.UserRepository.FindForPunchIn(userContext.Id);

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

                    var distance = userCoord.Distance(companyCoord);

                    if (distance > user.Company.CompanyGeolocation.RadiusMeters)
                    {
                        return PunchInResponse.OFR;
                    }
                }
                else if (!userContext.IsAllowedBypassCoord && dto is { Latitude: 0, Longitude: 0 })
                {
                    throw new BadHttpRequestException("Ocorreu um erro");
                }


                var workShifts = user.UserShifts?.Find(es => es.IsActive)?.WorkShift;

                if (workShifts != null)
                {
                    if (user.UserFaces != null)
                    {
                        var faceMatchs = await faceRecognitionIntegration.ChecksIfFaceMatchs(
                            dto.ImageBase64,
                            user.UserFaces.First().FaceTemplate
                        );

                        if (!faceMatchs)
                        {
                            return PunchInResponse.FNC;
                        }

                        Boolean timeCheked = CheckTime(WorkShiftMapper.FromWorkShift(workShifts),
                            TimeOnly.FromDateTime(dto.TimePunched), dto.Type);

                        var userAttendance =
                            UserAttendanceMapper.UserAttendanceFromDto(dto, userContext.Id,
                                timeCheked, timeCheked
                                    ? null
                                    : TimeOnly.FromTimeSpan(TimeOnly.FromDateTime(dto.TimePunched) -
                                                            TimeOnly.FromDateTime(DateTime.UtcNow)));

                        unitOfWork.UserAttendanceRepository.Create(userAttendance);

                        await unitOfWork.Save();

                        return PunchInResponse.SCC;
                    }
                }
            }

            throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
        }

        ///<summary>
        ///Operation for refresh information from user
        ///</summary>
        public async Task<UserInfoLoginDto> RefreshUserInformation(UserContext user)
        {
            TreatedUserInformationDto treatUser =
                await unitOfWork.UserRepository.FindUserInformation(user.Email!)! ??
                throw new InvalidOperationException("Erro interno! Comunique o Suporte.");
            return AuthMapper.MapUserToUserInfoLoginDto(treatUser);
        }

        ///<summary>
        ///Operation for punch for other user
        ///</summary>
        public async Task PunchForUser(PunchForUserDto dto, UserContext user)
        {
            UserAttendance userAttendance = UserAttendanceMapper.UserAttendanceFromDto(dto, user.Id);
            unitOfWork.UserAttendanceRepository.Create(userAttendance);
            await unitOfWork.Save();
        }

        ///<summary>
        ///Operation for check the time that user is punching, and return true or false, depends on time of WorkShift
        ///</summary>
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

        ///<summary>
        ///Operation for check if user can punch out in correctly time
        ///</summary>
        private Boolean CheckIfCanOut(WorkShiftPunchDto ws, TimeOnly time, PunchType type)
        {
            return ws.EndTime.AddMinutes(-ws.ToleranceMinutes) <= time;
        }
    }
}
