using DeltaFour.Application.Dtos;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Application.Emails;
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
using Serilog;
using System.Globalization;

namespace DeltaFour.Application.Services
{
    public class UserService(
        IUnitOfWork unitOfWork,
        IFaceRecognitionIntegration faceRecognitionIntegration,
        IPasswordService passwordService,
        PunctualityMetricsService punctualityMetricsService,
        NotificationService notificationService,
        IEmailSender emailSender
        )
    {
        private readonly String allowedHost = Environment.GetEnvironmentVariable("ALLOWED_HOST");

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
                    // A senha não é mais informada pelo cliente: geramos uma forte,
                    // persistimos o hash e enviamos a senha em texto claro por e-mail.
                    var generatedPassword = passwordService.GenerateStrong();
                    dto.Password = passwordService.Hash(generatedPassword);

                    var user = UserMapper.FromCreateDto(dto, role.Id, userAuthenticated);

                    unitOfWork.UserRepository.Create(user);

                    if (!dto.IsAllowedBypassFacial)
                    {
                        var embedding = await faceRecognitionIntegration.GetFaceEmbeddings(dto.ImageBase64);
                        var userFace = new UserFace(user.Id, embedding, userAuthenticated.Id);

                        unitOfWork.UserFaceRepository.Create(userFace);
                    }

                    var userShifts = new List<UserShift>();

                    foreach (var shift in dto.UserShift)
                    {
                        userShifts.Add(ShiftMapper.FromCreateUserDto(shift, user.Id, userAuthenticated.Id));
                    }

                    unitOfWork.UserShiftRepository.CreateAll(userShifts);

                    await unitOfWork.Save();

                    // Envio das credenciais não deve derrubar a criação já persistida.
                    try
                    {
                        await SendNewEmployeeCredentialsEmailAsync(dto.Email!, dto.Name!, generatedPassword);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Falha ao enviar e-mail de credenciais para o funcionário {Email}", dto.Email);
                    }
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
        ///Operation for check if user can punch in web
        ///</summary>
        public async Task<Boolean> CanPunchWeb(CanPunchDto dto, UserContext user)
        {
            var workShift = await unitOfWork.WorkShiftRepository.GetByUserIdAndIsActive(user.Id, user.CompanyId);
            int amountAttendance = await unitOfWork.UserAttendanceRepository.AmountAttendanceIn(user.Id);

            if (workShift != null && amountAttendance == 0 && dto.TimePunched >=
                workShift.StartTime.AddMinutes(workShift.ToleranceMinutes).Add(TimeSpan.FromSeconds(1)))
            {
                return false;
            }

            if (workShift != null && amountAttendance == 0)
            {
                return CheckTime(workShift, dto.TimePunched, dto.PunchType);
            }

            if (workShift != null && amountAttendance > 0 &&
                workShift.EndTime.AddMinutes(-workShift.ToleranceMinutes).Add(TimeSpan.FromSeconds(-1)) <=
                dto.TimePunched && dto.TimePunched <= workShift.EndTime.AddMinutes(workShift.ToleranceMinutes)
                    .Add(TimeSpan.FromSeconds(1)))
            {
                return CheckTime(workShift, dto.TimePunched, dto.PunchType);
            }

            return true;
        }

        ///<summary>
        ///Operation for punch for user
        ///</summary>
        public async Task<String> PunchIn(PunchDto dto, UserContext userContext)
        {
            var user = await unitOfWork.UserRepository.FindForPunchIn(userContext.Id);

            if (user != null)
            {
                String? validation = await ValidationsPunchIn(dto, user);
                if (validation != null)
                {
                    return validation;
                }

                var workShifts = user.UserShifts?.Find(es => es.IsActive)?.WorkShift;

                if (workShifts != null)
                {
                    Boolean timeCheked = CheckTime(WorkShiftMapper.FromWorkShift(workShifts),
                        TimeOnly.FromDateTime(dto.TimePunched), dto.Type);

                    var userAttendance =
                        UserAttendanceMapper.UserAttendanceFromDto(dto, userContext.Id,
                            timeCheked, timeCheked
                                ? null
                                : TimeOnly.FromTimeSpan(TimeOnly.FromDateTime(dto.TimePunched) -
                                                        TimeOnly.FromDateTime(DateTime.UtcNow)));

                    userAttendance.Status = StatusAttendance.aprovado.ToString();

                    unitOfWork.UserAttendanceRepository.Create(userAttendance);

                    await unitOfWork.Save();
                    // Recalcula métricas de pontualidade automaticamente
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await punctualityMetricsService.RecalculateMetricsForUser(userContext.Id);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Erro ao recalcular métricas de pontualidade para usuário {UserId}", userContext.Id);
                        }
                    });

                    await NotifyPunchInIfNeeded(dto.Type, userContext.CompanyId, userContext.Id,
                        userContext.Name, userAttendance.IsLate, userAttendance.PunchTime, userAttendance.Id);

                    return PunchInResponse.SCC.Message();
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
            // Recalcula métricas de pontualidade automaticamente
            _ = Task.Run(async () =>
            {
                try
                {
                    await punctualityMetricsService.RecalculateMetricsForUser(dto.UserId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Erro ao recalcular métricas de pontualidade para usuário {UserId}", dto.UserId);
                }
            });

            if (dto.Type == PunchType.IN)
            {
                var target = await unitOfWork.UserRepository.Find(u => u.Id == dto.UserId);
                await NotifyPunchInIfNeeded(dto.Type, user.CompanyId, dto.UserId, target?.Name,
                    userAttendance.IsLate, userAttendance.PunchTime, userAttendance.Id);
            }
        }

        ///<summary>
        ///Cria e dispara a notificação de entrada (apenas para batidas do tipo IN).
        ///Best-effort: falhas de notificação nunca quebram o registro de ponto.
        ///</summary>
        private async Task NotifyPunchInIfNeeded(PunchType type, Guid companyId, Guid userId,
            String? userName, Boolean isLate, DateTime punchTime, Guid attendanceId)
        {
            if (type != PunchType.IN)
            {
                return;
            }

            try
            {
                await notificationService.NotifyPunchInAsync(companyId, userId, userName, isLate, punchTime, attendanceId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao criar notificação de ponto para usuário {UserId}", userId);
            }
        }

        ///<summary>
        ///Operation for punch for user in web by email (can be used for punch late, normal, etc).
        ///</summary>
        public async Task PunchByEmail(PunchByEmailDto dto, UserContext userContext)
        {
            User? user = await unitOfWork.UserRepository.FindByEmailForPunch(userContext.Email!);
            if (user != null)
            {
                String? validation = await ValidationsPunchIn(dto, user);
                if (validation != null)
                {
                    throw new BadHttpRequestException(validation);
                }

                // using var hash = SHA256.Create();
                // byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
                // var hashPassowrd = new StringBuilder();
                // foreach (byte b in bytes)
                // {
                //     hashPassowrd.Append(b.ToString("x2"));
                // }

                // if (user.Password.Equals(hashPassowrd.ToString()))
                // {
                var workShifts = user.UserShifts?.Find(es => es.IsActive)?.WorkShift;

                if (workShifts != null)
                {
                    Boolean timeChecked = CheckTime(WorkShiftMapper.FromWorkShift(workShifts),
                        TimeOnly.FromDateTime(dto.TimePunched), dto.Type);

                    String? filePath = null;

                    if (!string.IsNullOrWhiteSpace(dto.FileBase64))
                    {
                        string base64 = dto.FileBase64;
                        string? mimeType = null;

                        if (base64.Contains(","))
                        {
                            var parts = base64.Split(',', 2);

                            var metadata = parts[0];
                            base64 = parts[1];

                            mimeType = metadata
                                .Replace("data:", "")
                                .Replace(";base64", "");
                        }

                        byte[] fileBytes;

                        fileBytes = Convert.FromBase64String(base64);

                        string extension = mimeType.Split('/')[1];


                        string folderName = mimeType == "application/pdf"
                            ? "pdf"
                            : "Image";

                        string fileName = $"{Guid.NewGuid()}.{extension}";

                        string folderPath = Path.Combine(
                            "..",
                            folderName
                        );

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string fullPath = Path.Combine(folderPath, fileName);

                        await File.WriteAllBytesAsync(fullPath, fileBytes);

                        filePath = Path.Combine(folderName, fileName).Replace("\\", "/");
                    }

                    var userAttendance =
                        UserAttendanceMapper.UserAttendanceFromDto(dto, userContext.Id,
                            timeChecked, timeChecked
                                ? null
                                : TimeOnly.FromTimeSpan(TimeOnly.FromDateTime(dto.TimePunched) -
                                                        TimeOnly.FromDateTime(DateTime.UtcNow)),
                            filePath);

                    unitOfWork.UserAttendanceRepository.Create(userAttendance);

                    await unitOfWork.Save();

                    // Recalcula métricas de pontualidade automaticamente
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await punctualityMetricsService.RecalculateMetricsForUser(userContext.Id);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Erro ao recalcular métricas de pontualidade para usuário {UserId}", userContext.Id);
                        }
                    });

                    await NotifyPunchInIfNeeded(dto.Type, userContext.CompanyId, userContext.Id,
                        userContext.Name, userAttendance.IsLate, userAttendance.PunchTime, userAttendance.Id);
                }
                // }
                // else
                // {
                //     throw new BadHttpRequestException("Senha esta incorreta!");
                // }
            }
        }

        ///<summary>
        ///Operation for get all attendance of all employees from company
        ///</summary>
        public async Task<List<AllAttendanceByCompanyResponse>> GetAllAttendanceByCompany(Guid companyId)
        {
            return await unitOfWork.UserRepository.GetAllAttendanceByCompany(companyId);
        }

        ///<summary>
        ///Operation for get attendance dashboard data from company
        ///</summary>
        public async Task<AttendanceDashboardResponse> GetAttendanceDashboard(Guid companyId)
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var trendStart = new DateTime(today.Year, today.Month, 1).AddMonths(-7);

            var users = await unitOfWork.UserRepository.GetDashboardUsers(companyId);
            var activeUsers = users.Where(u => u.IsActive).ToList();
            var attendances = await unitOfWork.UserRepository.GetDashboardAttendances(
                companyId,
                trendStart,
                today.AddDays(1));

            var todayAttendances = attendances
                .Where(a => a.PunchTime >= today && a.PunchTime < today.AddDays(1))
                .ToList();
            var monthlyAttendances = attendances
                .Where(a => a.PunchTime >= monthStart && a.PunchTime < monthStart.AddMonths(1))
                .ToList();

            return new AttendanceDashboardResponse
            {
                Summary = new AttendanceDashboardSummaryDto
                {
                    ActiveEmployees = activeUsers.Count,
                    PunctualityRate = CalculatePunctualityRate(todayAttendances),
                    NoClockToday = activeUsers.Count(u => todayAttendances.All(a => a.UserId != u.Id)),
                    MonthlyOvertimeHours = CalculateMonthlyOvertimeMinutes(activeUsers, monthlyAttendances, today)
                },
                WeeklyPresence = BuildWeeklyPresence(attendances, today.AddDays(-27), today),
                TopLateEmployees = BuildTopLateEmployees(monthlyAttendances),
                PunctualityTrend = BuildPunctualityTrend(attendances, trendStart, monthStart)
            };
        }

        ///<summary>
        ///General validation for punch in (verify bypass)
        ///</summary>
        private async Task<String?> ValidationsPunchIn(PunchDto dto, User user)
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
                    user!.Company.CompanyGeolocation!.Coord.Longitude,
                    user.Company.CompanyGeolocation.Coord.Latitude));

                var distance = userCoord.Distance(companyCoord);

                if (distance > user.Company.CompanyGeolocation.RadiusMeters)
                {
                    return PunchInResponse.OFR.Message();
                }
            }
            else if (!user.IsAllowedBypassCoord && dto is { Latitude: 0, Longitude: 0 })
            {
                throw new BadHttpRequestException("Ocorreu um erro");
            }

            if (user is { IsAllowedBypassFacial: false, UserFaces: not null })
            {
                var faceMatchs = await faceRecognitionIntegration.ChecksIfFaceMatchs(
                    dto.ImageBase64,
                    user.UserFaces.First().FaceTemplate
                );

                if (!faceMatchs)
                {
                    return PunchInResponse.FNC.Message();
                }
            }

            return null;
        }

        public async Task UpdateStatusAttendance(UpdateStatusAttendanceDto dto, Guid attendanceId, UserContext userAuthenticated)
        {
            var attendance = await unitOfWork.UserAttendanceRepository.Find(a => a.Id == attendanceId);
            if (attendance == null)
            {
                throw new InvalidOperationException("Registro de ponto não encontrado.");
            }

            var timeSheet = await unitOfWork.TimeSheetRepository.FindByUserMonthYear(
                attendance.UserId, attendance.PunchTime.Month, attendance.PunchTime.Year);

            if (timeSheet is { SignedByEmployee: true, SignedByHR: true })
            {
                throw new InvalidOperationException("A folha de ponto já foi finalizada e não pode ser alterada.");
            }

            var oldStatus = attendance.Status ?? string.Empty;
            var newStatus = Enum.GetName(dto.Status) ?? string.Empty;

            attendance.Status = newStatus;
            attendance.UpdatedAt = DateTime.UtcNow;
            attendance.UpdatedBy = userAuthenticated.Id;
            unitOfWork.UserAttendanceRepository.Update(attendance);

            if (timeSheet != null)
            {
                unitOfWork.TimeSheetAuditRepository.Create(new TimeSheetAudit
                {
                    TimeSheetId = timeSheet.Id,
                    UserId = userAuthenticated.Id,
                    UserName = userAuthenticated.Name ?? string.Empty,
                    Operation = "UpdateAttendanceStatus",
                    OldValues = oldStatus,
                    NewValues = newStatus
                });
            }

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
        ///Envia ao novo funcionário a senha de acesso gerada pelo sistema.
        ///</summary>
        private async Task SendNewEmployeeCredentialsEmailAsync(String email, String name, String plainPassword)
        {
            var loginUrl = $"{allowedHost}/v1/login";

            var html = EmailBuilder.Create()
                .Preheader("Sua conta de acesso ao DeltaFour foi criada.")
                .Title($"Bem-vindo(a), {name}!")
                .Paragraph("Sua conta de acesso ao DeltaFour foi criada com sucesso. Utilize as credenciais abaixo para realizar o seu primeiro acesso:")
                .InfoRow("E-mail", email)
                .InfoRow("Senha temporária", plainPassword)
                .Button("Acessar a plataforma", loginUrl)
                .Note("Por segurança, no primeiro acesso você será solicitado a criar uma nova senha pessoal.")
                .Render();

            await emailSender.SendAsync(email, "Bem-vindo ao DeltaFour - Sua senha de acesso", html);
        }

        private async Task SendEmailRh(List<User> rhUsers, String latestUserName, String latestUserEmail)
        {
            var html = EmailBuilder.Create()
                .Preheader("Registro de atraso de colaborador.")
                .Title("Funcionário atrasado")
                .Paragraph("Identificamos um registro de atraso. Confira os detalhes do colaborador abaixo:")
                .InfoRow("Nome", latestUserName)
                .InfoRow("E-mail", latestUserEmail)
                .Note("Este alerta é gerado automaticamente com base nas regras de tolerância configuradas para o turno.")
                .Render();

            var recipients = rhUsers
                .Select(rhUser => rhUser.Email)
                .Where(emailAddress => !string.IsNullOrWhiteSpace(emailAddress))
                .Select(emailAddress => emailAddress!)
                .ToList();

            await emailSender.SendAsync(recipients, "Funcionário Atrasado", html);
        }

        ///<summary>
        ///Operation for check if user can punch out in correctly time
        ///</summary>
        private Boolean CheckIfCanOut(WorkShiftPunchDto ws, TimeOnly time, PunchType type)
        {
            return ws.EndTime.AddMinutes(-ws.ToleranceMinutes) <= time;
        }

        public async Task<List<UserSelectResponse>> GetAllSelect(Guid companyId)
        {
            var users = await unitOfWork.UserRepository.GetAllSelect(companyId);
            return users.Select(u => new UserSelectResponse
            {
                Id = u.Id.ToString(),
                Name = u.Name,
                DepartmentName = u.Department?.Name,
            }).ToList();
        }

        private static int CalculatePunctualityRate(List<UserAttendance> attendances)
        {
            if (attendances.Count == 0)
            {
                return 0;
            }

            var punctualCount = attendances.Count(a => !a.IsLate);
            return CalculatePercentage(punctualCount, attendances.Count);
        }

        private static List<AttendanceWeeklyPresenceDto> BuildWeeklyPresence(
            List<UserAttendance> attendances,
            DateTime weeklyStart,
            DateTime today)
        {
            var weeklyPresence = new List<AttendanceWeeklyPresenceDto>();

            for (var week = 0; week < 4; week++)
            {
                var start = weeklyStart.Date.AddDays(week * 7);
                var end = week == 3 ? today.Date.AddDays(1) : start.AddDays(7);
                var weekAttendances = attendances
                    .Where(a => a.PunchTime >= start && a.PunchTime < end)
                    .ToList();

                if (weekAttendances.Count == 0)
                {
                    continue;
                }

                var punctual = CalculatePercentage(weekAttendances.Count(a => !a.IsLate), weekAttendances.Count);

                weeklyPresence.Add(new AttendanceWeeklyPresenceDto
                {
                    WeekLabel = $"Sem {weeklyPresence.Count + 1}",
                    Punctual = punctual,
                    Late = 100 - punctual
                });
            }

            return weeklyPresence;
        }

        private static List<AttendanceTopLateEmployeeDto> BuildTopLateEmployees(List<UserAttendance> monthlyAttendances)
        {
            return monthlyAttendances
                .Where(a => a.IsLate && a.User != null)
                .GroupBy(a => new { a.UserId, a.User!.Name })
                .Select(g => new AttendanceTopLateEmployeeDto
                {
                    Name = g.Key.Name,
                    LateCount = g.Count()
                })
                .OrderByDescending(e => e.LateCount)
                .ThenBy(e => e.Name)
                .Take(5)
                .ToList();
        }

        private static List<AttendancePunctualityTrendDto> BuildPunctualityTrend(
            List<UserAttendance> attendances,
            DateTime trendStart,
            DateTime currentMonthStart)
        {
            var trend = new List<AttendancePunctualityTrendDto>();

            for (var monthStart = trendStart; monthStart <= currentMonthStart; monthStart = monthStart.AddMonths(1))
            {
                var nextMonth = monthStart.AddMonths(1);
                var monthAttendances = attendances
                    .Where(a => a.PunchTime >= monthStart && a.PunchTime < nextMonth)
                    .ToList();

                trend.Add(new AttendancePunctualityTrendDto
                {
                    Month = GetShortMonthName(monthStart.Month),
                    Rate = CalculatePunctualityRate(monthAttendances)
                });
            }

            return trend;
        }

        private static int CalculateMonthlyOvertimeMinutes(
            List<User> activeUsers,
            List<UserAttendance> monthlyAttendances,
            DateTime today)
        {
            var totalMinutes = 0;

            foreach (var user in activeUsers)
            {
                var workShift = user.UserShifts?
                    .FirstOrDefault(s => s.IsActive)
                    ?.WorkShift;

                if (workShift == null)
                {
                    continue;
                }

                var expectedHours = TimeSheetCalculator.CalculateShiftDuration(
                    workShift.StartTime,
                    workShift.EndTime);

                var attendancesByDay = monthlyAttendances
                    .Where(a => a.UserId == user.Id)
                    .GroupBy(a => DateOnly.FromDateTime(a.PunchTime));

                foreach (var dayAttendancesGroup in attendancesByDay)
                {
                    var date = dayAttendancesGroup.Key;
                    if (date.ToDateTime(TimeOnly.MinValue) > today)
                    {
                        continue;
                    }

                    var dayAttendances = dayAttendancesGroup
                        .OrderBy(a => a.PunchTime)
                        .ToList();
                    var firstEntry = TimeSheetCalculator.GetFirstEntry(dayAttendances);
                    var lastExit = TimeSheetCalculator.GetLastExit(dayAttendances);
                    var workedHours = TimeSheetCalculator.CalculateWorkedHours(firstEntry, lastExit);
                    var expectedDayHours = TimeSheetCalculator.IsWorkday(date.DayOfWeek)
                        ? expectedHours
                        : TimeSpan.Zero;
                    var extraHours = workedHours - expectedDayHours;

                    if (extraHours > TimeSpan.Zero)
                    {
                        totalMinutes += (int)Math.Round(extraHours.TotalMinutes);
                    }
                }
            }

            return totalMinutes;
        }

        private static int CalculatePercentage(int value, int total)
        {
            if (total == 0)
            {
                return 0;
            }

            return (int)Math.Round(value * 100.0 / total, MidpointRounding.AwayFromZero);
        }

        private static string GetShortMonthName(int month)
        {
            var name = new CultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedMonthName(month);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Replace(".", ""));
        }
    }
}