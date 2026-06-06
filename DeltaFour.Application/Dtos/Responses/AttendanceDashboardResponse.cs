namespace DeltaFour.Application.Dtos.Responses;

public class AttendanceDashboardResponse
{
    public AttendanceDashboardSummaryDto Summary { get; set; }

    public List<AttendanceWeeklyPresenceDto> WeeklyPresence { get; set; }

    public List<AttendanceTopLateEmployeeDto> TopLateEmployees { get; set; }

    public List<AttendancePunctualityTrendDto> PunctualityTrend { get; set; }
}

public class AttendanceDashboardSummaryDto
{
    public int ActiveEmployees { get; set; }

    public int PunctualityRate { get; set; }

    public int NoClockToday { get; set; }

    public int MonthlyOvertimeHours { get; set; }
}

public class AttendanceWeeklyPresenceDto
{
    public string WeekLabel { get; set; }

    public int Punctual { get; set; }

    public int Late { get; set; }
}

public class AttendanceTopLateEmployeeDto
{
    public string Name { get; set; }

    public int LateCount { get; set; }
}

public class AttendancePunctualityTrendDto
{
    public string Month { get; set; }
    
    public int Rate { get; set; }
}
