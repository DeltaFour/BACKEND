using DeltaFour.Application.Documents;
using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
    /// Assina a folha de ponto pelo funcionário
    /// </summary>
    Task SignByEmployeeAsync(Guid timeSheetId, Guid employeeId);

    /// <summary>
    /// Assina a folha de ponto pelo RH
    /// </summary>
    Task SignByHRAsync(Guid timeSheetId, Guid hrUserId, string hrUserName);

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
    private readonly IUnitOfWork _unitOfWork;

    public TimeSheetPdfService(
        IUserRepository userRepository,
        IUserAttendanceRepository attendanceRepository,
        IUserShiftRepository userShiftRepository,
        ICompanyRepository companyRepository,
        ITimeSheetRepository timeSheetRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _attendanceRepository = attendanceRepository;
        _userShiftRepository = userShiftRepository;
        _companyRepository = companyRepository;
        _timeSheetRepository = timeSheetRepository;
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

    public async Task SignByEmployeeAsync(Guid timeSheetId, Guid employeeId)
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

        if (timeSheet.SignedByEmployee)
        {
            throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo funcionário.");
        }

        timeSheet.SignedByEmployee = true;
        timeSheet.EmployeeSignedAt = DateTime.UtcNow;

        _timeSheetRepository.Update(timeSheet);
        await _unitOfWork.Save();
    }

    public async Task SignByHRAsync(Guid timeSheetId, Guid hrUserId, string hrUserName)
    {
        var timeSheet = await _timeSheetRepository.Find(t => t.Id == timeSheetId);
        if (timeSheet == null)
        {
            throw new ArgumentException("Folha de ponto não encontrada.", nameof(timeSheetId));
        }

        if (timeSheet.SignedByHR)
        {
            throw new InvalidOperationException("Esta folha de ponto já foi assinada pelo RH.");
        }

        timeSheet.SignedByHR = true;
        timeSheet.HRSignedAt = DateTime.UtcNow;
        timeSheet.SignedByHRUserId = hrUserId;
        timeSheet.SignedByHRUserName = hrUserName;

        _timeSheetRepository.Update(timeSheet);
        await _unitOfWork.Save();
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
