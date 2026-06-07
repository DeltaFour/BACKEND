using DeltaFour.Application.Documents;
using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DeltaFour.Application.Services;

/// <summary>
/// Interface para o serviço de geração de folha de ponto em PDF
/// </summary>
public interface ITimeSheetPdfService
{
    /// <summary>
    /// Gera a folha de ponto em PDF para o funcionário e período especificado
    /// </summary>
    Task<byte[]> GenerateTimeSheetAsync(TimeSheetRequestDto request);

    /// <summary>
    /// Gera os dados da folha de ponto sem criar o PDF
    /// </summary>
    Task<TimeSheetDataDto> GetTimeSheetDataAsync(TimeSheetRequestDto request);

    /// <summary>
    /// Solicita a assinatura da folha de ponto pelo funcionário enviando o código por e-mail
    /// </summary>
    Task RequestEmployeeSignatureAsync(Guid timeSheetId, Guid employeeId);

    /// <summary>
    /// Solicita a assinatura da folha de ponto pelo RH enviando o código por e-mail
    /// </summary>
    Task RequestHRSignatureAsync(Guid timeSheetId, Guid hrUserId);

    /// <summary>
    /// Efetiva a assinatura eletrônica da folha de ponto a partir do código recebido por e-mail
    /// </summary>
    Task ConfirmSignatureAsync(string token, string ipAddress);

    /// <summary>
    /// Obtém o histórico de assinaturas da folha de ponto
    /// </summary>
    Task<TimeSheetSignatureHistoryDto> GetSignatureHistoryAsync(Guid timeSheetId);

    /// <summary>
    /// Obtém o histórico de alterações da folha de ponto
    /// </summary>
    Task<List<TimeSheetAuditDto>> GetAuditHistoryAsync(Guid timeSheetId);

    /// <summary>
    /// Obtém o registro da folha de ponto
    /// </summary>
    Task<TimeSheet?> GetTimeSheetRecordAsync(Guid userId, int month, int year);

    /// <summary>
    /// Lista folhas de ponto com filtros opcionais
    /// </summary>
    Task<List<TimeSheetListItemDto>> ListTimeSheetsAsync(Guid? userId, int? month, int? year);
}

/// <summary>
/// Serviço para geração de folha de ponto em PDF
/// </summary>
public class TimeSheetPdfService : ITimeSheetPdfService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAttendanceRepository _attendanceRepository;
    private readonly IUserShiftRepository _userShiftRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ITimeSheetRepository _timeSheetRepository;
    private readonly ITimeSheetSignatureRepository _timeSheetSignatureRepository;
    private readonly ITimeSheetSignatureTokenRepository _timeSheetSignatureTokenRepository;
    private readonly ITimeSheetAuditRepository _timeSheetAuditRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly string host = Environment.GetEnvironmentVariable("EMAIL_HOST");
    private readonly int port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT"));
    private readonly string username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
    private readonly string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
    private readonly string fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL");
    private readonly string fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME");
    private readonly string allowedHost = Environment.GetEnvironmentVariable("ALLOWED_HOST");

    public TimeSheetPdfService(
        IUserRepository userRepository,
        IUserAttendanceRepository attendanceRepository,
        IUserShiftRepository userShiftRepository,
        ICompanyRepository companyRepository,
        ITimeSheetRepository timeSheetRepository,
        ITimeSheetSignatureRepository timeSheetSignatureRepository,
        ITimeSheetSignatureTokenRepository timeSheetSignatureTokenRepository,
        ITimeSheetAuditRepository timeSheetAuditRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _attendanceRepository = attendanceRepository;
        _userShiftRepository = userShiftRepository;
        _companyRepository = companyRepository;
        _timeSheetRepository = timeSheetRepository;
        _timeSheetSignatureRepository = timeSheetSignatureRepository;
        _timeSheetSignatureTokenRepository = timeSheetSignatureTokenRepository;
        _timeSheetAuditRepository = timeSheetAuditRepository;
        _unitOfWork = unitOfWork;

        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateTimeSheetAsync(TimeSheetRequestDto request)
    {
        var data = await GetTimeSheetDataAsync(request);
        var document = new TimeSheetDocument(data);
        return document.GeneratePdf();
    }

    public async Task<TimeSheetDataDto> GetTimeSheetDataAsync(TimeSheetRequestDto request)
    {

        if (request.Month < 1 || request.Month > 12)
        {
            throw new ArgumentException("Mês inválido. Deve ser entre 1 e 12.", nameof(request.Month));
        }

        if (request.Year < 2000 || request.Year > 2100)
        {
            throw new ArgumentException("Ano inválido.", nameof(request.Year));
        }


        var user = await _userRepository.FindIncluding(request.UserId);
        if (user == null)
        {
            throw new ArgumentException("Funcionário não encontrado.", nameof(request.UserId));
        }


        var company = await _companyRepository.Find(c => c.Id == user.CompanyId);
        if (company == null)
        {
            throw new ArgumentException("Empresa não encontrada.");
        }

        var timeSheet = await GetOrCreateTimeSheetAsync(request.UserId, request.Month, request.Year);

        var userShift = await _userShiftRepository.Find(
            us => us.UserId == request.UserId && us.IsActive);

        WorkShift? workShift = userShift?.WorkShift;


        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);
        var today = DateTime.Today;
        var isMonthComplete = endDate.Date <= today;


        var attendances = await _attendanceRepository.FindAll(
            a => a.UserId == request.UserId &&
                 a.PunchTime >= startDate &&
                 a.PunchTime <= endDate);


        var attendancesByDay = TimeSheetCalculator.GroupAttendancesByDay(attendances);

        var expectedHoursPerDay = workShift != null
            ? TimeSheetCalculator.CalculateShiftDuration(workShift.StartTime, workShift.EndTime)
            : TimeSpan.FromHours(8);


        var days = GenerateDaysOfMonth(
            request.Year,
            request.Month,
            attendancesByDay,
            expectedHoursPerDay,
            today);


        var summary = TimeSheetCalculator.CalculateSummary(days, isMonthComplete);

        return new TimeSheetDataDto
        {
            TimeSheetId = timeSheet.Id,
            Company = new TimeSheetCompanyDto
            {
                Name = company.Name ?? "Não informado",
                Cnpj = TimeSheetCalculator.FormatCnpj(company.Cnpj),
                FullAddress = TimeSheetCalculator.FormatFullAddress(company.Address)
            },
            Employee = new TimeSheetEmployeeDto
            {
                Id = user.Id,
                Name = user.Name ?? "Não informado",
                Role = user.Role?.Name ?? "Não informado",
                ShiftName = workShift != null
                    ? TimeSheetCalculator.GetShiftTypeName(workShift.ShiftType)
                    : "Não definido",
                ShiftStartTime = workShift?.StartTime ?? new TimeOnly(8, 0),
                ShiftEndTime = workShift?.EndTime ?? new TimeOnly(17, 0)
            },
            Period = TimeSheetCalculator.FormatPeriod(request.Month, request.Year),
            Month = request.Month,
            Year = request.Year,
            Days = days,
            Summary = summary,
            Signature = new TimeSheetSignatureDto
            {
                SignedByEmployee = timeSheet.SignedByEmployee,
                EmployeeName = user.Name ?? "Não informado",
                EmployeeSignedAt = timeSheet.EmployeeSignedAt,
                SignedByHR = timeSheet.SignedByHR,
                HRSignerName = timeSheet.SignedByHRUserName ?? string.Empty,
                HRSignedAt = timeSheet.HRSignedAt
            },
            GeneratedAt = DateTime.Now
        };
    }

    public async Task RequestEmployeeSignatureAsync(Guid timeSheetId, Guid employeeId)
    {
        var timeSheet = await _timeSheetRepository.Find(t => t.Id == timeSheetId);
        if (timeSheet == null)
        {
            throw new ArgumentException("Folha de ponto não encontrada.", nameof(timeSheetId));
        }

        if (timeSheet.UserId != employeeId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para assinar esta folha de ponto.");
        }

        EnsureNotFinalized(timeSheet);

        if (timeSheet.SignedByEmployee)
        {
            throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo funcionário.");
        }

        var employee = await _userRepository.Find(u => u.Id == timeSheet.UserId);
        if (employee == null)
        {
            throw new ArgumentException("Funcionário não encontrado.");
        }

        await CreateAndSendSignatureRequestAsync(timeSheet, SignerType.Employee, employee.Id, employee.Email);
    }

    public async Task RequestHRSignatureAsync(Guid timeSheetId, Guid hrUserId)
    {
        var timeSheet = await _timeSheetRepository.Find(t => t.Id == timeSheetId);
        if (timeSheet == null)
        {
            throw new ArgumentException("Folha de ponto não encontrada.", nameof(timeSheetId));
        }

        EnsureNotFinalized(timeSheet);

        if (timeSheet.SignedByHR)
        {
            throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo RH.");
        }

        var hrUser = await _userRepository.Find(u => u.Id == hrUserId);
        if (hrUser == null)
        {
            throw new ArgumentException("Usuário do RH não encontrado.");
        }

        await CreateAndSendSignatureRequestAsync(timeSheet, SignerType.HR, hrUser.Id, hrUser.Email);
    }

    public async Task ConfirmSignatureAsync(string token, string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Código de assinatura inválido.", nameof(token));
        }

        var signatureToken = await _timeSheetSignatureTokenRepository.FindByToken(token);
        if (signatureToken == null)
        {
            throw new ArgumentException("Código de assinatura inválido.", nameof(token));
        }

        if (signatureToken.UsedAtUtc != null)
        {
            throw new InvalidOperationException("Este código de assinatura já foi utilizado.");
        }

        if (signatureToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new InvalidOperationException("O código de assinatura expirou.");
        }

        var timeSheet = await _timeSheetRepository.Find(t => t.Id == signatureToken.TimeSheetId);
        if (timeSheet == null)
        {
            throw new ArgumentException("Folha de ponto não encontrada.");
        }

        EnsureNotFinalized(timeSheet);

        var employee = await _userRepository.FindIncluding(timeSheet.UserId);
        if (employee == null)
        {
            throw new ArgumentException("Funcionário não encontrado.");
        }

        var data = await GetTimeSheetDataAsync(new TimeSheetRequestDto
        {
            UserId = timeSheet.UserId,
            Month = timeSheet.Month,
            Year = timeSheet.Year
        });

        var hash = ComputeTimeSheetHash(data);
        var now = DateTime.UtcNow;

        string signerName;
        string signerCpf;
        string signerEmail;

        if (signatureToken.SignerType == SignerType.Employee)
        {
            if (timeSheet.SignedByEmployee)
            {
                throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo funcionário.");
            }

            signerName = employee.Name ?? string.Empty;
            signerCpf = employee.Cpf ?? string.Empty;
            signerEmail = employee.Email;

            timeSheet.SignedByEmployee = true;
            timeSheet.EmployeeSignedAt = now;
        }
        else
        {
            if (timeSheet.SignedByHR)
            {
                throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo RH.");
            }

            var hrUser = await _userRepository.Find(u => u.Id == signatureToken.SignerUserId);
            if (hrUser == null)
            {
                throw new ArgumentException("Usuário do RH não encontrado.");
            }

            var company = await _companyRepository.Find(c => c.Id == employee.CompanyId);

            signerName = hrUser.Name ?? string.Empty;
            signerCpf = company?.Cnpj ?? string.Empty;
            signerEmail = hrUser.Email;

            timeSheet.SignedByHR = true;
            timeSheet.HRSignedAt = now;
            timeSheet.SignedByHRUserId = hrUser.Id;
            timeSheet.SignedByHRUserName = signerName;
        }

        _timeSheetSignatureRepository.Create(new TimeSheetSignature
        {
            TimeSheetId = timeSheet.Id,
            SignerType = signatureToken.SignerType,
            SignerUserId = signatureToken.SignerUserId,
            SignerName = signerName,
            SignerCpf = signerCpf,
            SignerEmail = signerEmail,
            SignedAtUtc = now,
            SignerIp = ipAddress ?? string.Empty,
            TimeSheetHash = hash
        });

        signatureToken.UsedAtUtc = now;
        _timeSheetSignatureTokenRepository.Update(signatureToken);

        _timeSheetRepository.Update(timeSheet);
        await _unitOfWork.Save();
    }

    public async Task<TimeSheetSignatureHistoryDto> GetSignatureHistoryAsync(Guid timeSheetId)
    {
        var signatures = await _timeSheetSignatureRepository.FindByTimeSheet(timeSheetId);
        var tokens = await _timeSheetSignatureTokenRepository.FindByTimeSheet(timeSheetId);

        return new TimeSheetSignatureHistoryDto
        {
            Signatures = signatures.Select(s => new TimeSheetSignatureItemDto
            {
                SignerType = s.SignerType.ToString(),
                SignerName = s.SignerName,
                SignerCpf = s.SignerCpf,
                SignerEmail = s.SignerEmail,
                SignedAtUtc = s.SignedAtUtc,
                SignerIp = s.SignerIp,
                TimeSheetHash = s.TimeSheetHash
            }).ToList(),
            Requests = tokens.Select(t => new TimeSheetSignatureRequestItemDto
            {
                SignerType = t.SignerType.ToString(),
                Email = t.Email,
                CreatedAt = t.CreatedAt,
                ExpiresAtUtc = t.ExpiresAtUtc,
                UsedAtUtc = t.UsedAtUtc
            }).ToList()
        };
    }

    public async Task<List<TimeSheetAuditDto>> GetAuditHistoryAsync(Guid timeSheetId)
    {
        var audits = await _timeSheetAuditRepository.FindByTimeSheet(timeSheetId);

        return audits.Select(a => new TimeSheetAuditDto
        {
            Operation = a.Operation,
            UserName = a.UserName,
            OldValues = a.OldValues,
            NewValues = a.NewValues,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    private async Task CreateAndSendSignatureRequestAsync(
        TimeSheet timeSheet,
        SignerType signerType,
        Guid signerUserId,
        string email)
    {
        var token = GenerateToken();

        _timeSheetSignatureTokenRepository.Create(new TimeSheetSignatureToken
        {
            TimeSheetId = timeSheet.Id,
            SignerType = signerType,
            SignerUserId = signerUserId,
            Token = token,
            Email = email,
            ExpiresAtUtc = DateTime.UtcNow.AddHours(48)
        });

        await _unitOfWork.Save();

        await SendSignatureEmailAsync(email, token);
    }

    private static string GenerateToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    }

    private static void EnsureNotFinalized(TimeSheet timeSheet)
    {
        if (timeSheet.SignedByEmployee && timeSheet.SignedByHR)
        {
            throw new InvalidOperationException("A folha de ponto já foi finalizada e não pode ser alterada.");
        }
    }

    private static string ComputeTimeSheetHash(TimeSheetDataDto data)
    {
        var payload = JsonSerializer.Serialize(new
        {
            data.Employee,
            data.Company,
            data.Period,
            data.Month,
            data.Year,
            data.Days,
            data.Summary
        });

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task SendSignatureEmailAsync(string email, string token)
    {
        var link = $"{allowedHost}/folha-ponto/assinatura?token={token}";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Assinatura da Folha de Ponto";

        message.Body = new BodyBuilder
        {
            HtmlBody = $"""
                            <h2>Assinatura da Folha de Ponto</h2>

                            <p>Foi solicitada a sua assinatura eletrônica da folha de ponto.</p>

                            <p>Utilize o código abaixo para confirmar a assinatura:</p>

                            <p><strong>{token}</strong></p>

                            <p>Ou acesse diretamente: <a href="{link}">{link}</a></p>

                            <p>Este código é de uso único e expira em 48 horas.</p>
                        """
        }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task<TimeSheet?> GetTimeSheetRecordAsync(Guid userId, int month, int year)
    {
        return await _timeSheetRepository.FindByUserMonthYear(userId, month, year);
    }

    public async Task<List<TimeSheetListItemDto>> ListTimeSheetsAsync(Guid? userId, int? month, int? year)
    {
        var records = await _timeSheetRepository.FindAll(t =>
            (!userId.HasValue || t.UserId == userId.Value) &&
            (!month.HasValue || t.Month == month.Value) &&
            (!year.HasValue || t.Year == year.Value));

        return records.Select(r => new TimeSheetListItemDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User?.Name ?? string.Empty,
            Month = r.Month,
            Year = r.Year,
            SignedByEmployee = r.SignedByEmployee,
            SignedByHR = r.SignedByHR,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    private async Task<TimeSheet> GetOrCreateTimeSheetAsync(Guid userId, int month, int year)
    {
        var existingTimeSheet = await _timeSheetRepository.FindByUserMonthYear(userId, month, year);

        if (existingTimeSheet != null)
        {
            return existingTimeSheet;
        }

        var newTimeSheet = new TimeSheet
        {
            UserId = userId,
            Month = month,
            Year = year,
            SignedByEmployee = false,
            SignedByHR = false
        };

        _timeSheetRepository.Create(newTimeSheet);
        await _unitOfWork.Save();

        return newTimeSheet;
    }

    private static List<TimeSheetDayDto> GenerateDaysOfMonth(
        int year,
        int month,
        Dictionary<DateOnly, List<UserAttendance>> attendancesByDay,
        TimeSpan expectedHoursPerDay,
        DateTime today)
    {
        var days = new List<TimeSheetDayDto>();
        var daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(year, month, day);
            var dateTime = date.ToDateTime(TimeOnly.MinValue);
            var isFutureDay = dateTime > today;
            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday ||
                            date.DayOfWeek == DayOfWeek.Sunday;

            attendancesByDay.TryGetValue(date, out var dayAttendances);
            var hasApprovedRecords = dayAttendances != null && dayAttendances.Any();

            TimeOnly? firstEntry = null;
            TimeOnly? lastExit = null;

            if (hasApprovedRecords)
            {
                firstEntry = TimeSheetCalculator.GetFirstEntry(dayAttendances!);
                lastExit = TimeSheetCalculator.GetLastExit(dayAttendances!);
            }

            var workedHours = TimeSheetCalculator.CalculateWorkedHours(firstEntry, lastExit);
            var isDayOff = isWeekend;
            var expectedHours = isDayOff || isFutureDay ? TimeSpan.Zero : expectedHoursPerDay;
            var balance = isDayOff || isFutureDay ? TimeSpan.Zero : workedHours - expectedHours;
            var isAbsent = !isFutureDay && !isDayOff && !hasApprovedRecords;
            var isIncomplete = hasApprovedRecords && (!firstEntry.HasValue || !lastExit.HasValue);

            var observation = TimeSheetCalculator.GetDayObservation(
                isDayOff, isFutureDay, hasApprovedRecords, firstEntry, lastExit);

            days.Add(new TimeSheetDayDto
            {
                Date = date,
                DayOfWeek = TimeSheetCalculator.GetDayOfWeekName(date.DayOfWeek),
                FirstEntry = firstEntry,
                LastExit = lastExit,
                WorkedHours = workedHours,
                ExpectedHours = expectedHours,
                Balance = balance,
                Observation = observation,
                IsDayOff = isDayOff,
                IsFutureDay = isFutureDay,
                IsAbsent = isAbsent,
                IsIncomplete = isIncomplete
            });
        }

        return days;
    }
}
