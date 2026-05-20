using DeltaFour.Application.Documents;
using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
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
    /// <param name="request">Dados da requisição contendo ID do funcionário, mês e ano</param>
    /// <returns>Bytes do PDF gerado</returns>
    Task<byte[]> GenerateTimeSheetAsync(TimeSheetRequestDto request);

    /// <summary>
    /// Gera os dados da folha de ponto sem criar o PDF
    /// </summary>
    /// <param name="request">Dados da requisição contendo ID do funcionário, mês e ano</param>
    /// <returns>Dados estruturados da folha de ponto</returns>
    Task<TimeSheetDataDto> GetTimeSheetDataAsync(TimeSheetRequestDto request);
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

    public TimeSheetPdfService(
        IUserRepository userRepository,
        IUserAttendanceRepository attendanceRepository,
        IUserShiftRepository userShiftRepository,
        ICompanyRepository companyRepository)
    {
        _userRepository = userRepository;
        _attendanceRepository = attendanceRepository;
        _userShiftRepository = userShiftRepository;
        _companyRepository = companyRepository;

        // Configura a licença do QuestPDF como Community
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
        // Validação básica
        if (request.Month < 1 || request.Month > 12)
        {
            throw new ArgumentException("Mês inválido. Deve ser entre 1 e 12.", nameof(request.Month));
        }

        if (request.Year < 2000 || request.Year > 2100)
        {
            throw new ArgumentException("Ano inválido.", nameof(request.Year));
        }

        // Busca o usuário com suas relações
        var user = await _userRepository.FindIncluding(request.UserId);
        if (user == null)
        {
            throw new ArgumentException("Funcionário não encontrado.", nameof(request.UserId));
        }

        // Busca a empresa com endereço
        var company = await _companyRepository.Find(c => c.Id == user.CompanyId);
        if (company == null)
        {
            throw new ArgumentException("Empresa não encontrada.");
        }

        // Busca o turno ativo do funcionário
        var userShift = await _userShiftRepository.Find(
            us => us.UserId == request.UserId && us.IsActive);

        WorkShift? workShift = userShift?.WorkShift;

        // Determina o período
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);
        var today = DateTime.Today;
        var isMonthComplete = endDate.Date <= today;

        // Busca os registros de ponto do período
        var attendances = await _attendanceRepository.FindAll(
            a => a.UserId == request.UserId &&
                 a.PunchTime >= startDate &&
                 a.PunchTime <= endDate);

        // Agrupa por dia (apenas aprovados)
        var attendancesByDay = TimeSheetCalculator.GroupAttendancesByDay(attendances);

        // Calcula horas esperadas por dia de trabalho
        var expectedHoursPerDay = workShift != null 
            ? TimeSheetCalculator.CalculateShiftDuration(workShift.StartTime, workShift.EndTime)
            : TimeSpan.FromHours(8); // Default de 8 horas

        // Gera a lista de dias do mês
        var days = GenerateDaysOfMonth(
            request.Year, 
            request.Month, 
            attendancesByDay, 
            expectedHoursPerDay, 
            today);

        // Calcula os totalizadores
        var summary = TimeSheetCalculator.CalculateSummary(days, isMonthComplete);

        // Monta o DTO de retorno
        return new TimeSheetDataDto
        {
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
            GeneratedAt = DateTime.Now
        };
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

            // Verifica se há registros aprovados para o dia
            attendancesByDay.TryGetValue(date, out var dayAttendances);
            var hasApprovedRecords = dayAttendances != null && dayAttendances.Any();

            // Obtém entrada e saída
            TimeOnly? firstEntry = null;
            TimeOnly? lastExit = null;

            if (hasApprovedRecords)
            {
                firstEntry = TimeSheetCalculator.GetFirstEntry(dayAttendances!);
                lastExit = TimeSheetCalculator.GetLastExit(dayAttendances!);
            }

            // Calcula horas trabalhadas
            var workedHours = TimeSheetCalculator.CalculateWorkedHours(firstEntry, lastExit);

            // Define se é dia de folga (finais de semana são considerados folga por padrão)
            var isDayOff = isWeekend;

            // Define horas esperadas (zero para folgas e dias futuros)
            var expectedHours = isDayOff || isFutureDay ? TimeSpan.Zero : expectedHoursPerDay;

            // Calcula saldo (apenas para dias úteis passados)
            var balance = isDayOff || isFutureDay ? TimeSpan.Zero : workedHours - expectedHours;

            // Determina se é falta (dia útil passado sem registros aprovados)
            var isAbsent = !isFutureDay && !isDayOff && !hasApprovedRecords;

            // Determina se está incompleto (tem entrada mas não tem saída ou vice-versa)
            var isIncomplete = hasApprovedRecords && (!firstEntry.HasValue || !lastExit.HasValue);

            // Gera observação
            var observation = TimeSheetCalculator.GetDayObservation(
                isDayOff, 
                isFutureDay, 
                hasApprovedRecords, 
                firstEntry, 
                lastExit);

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
