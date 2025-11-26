using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Dto
{
    public sealed class ApiUserDto
    {
        public string Name { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public string ShiftType { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public string? LastPunchType { get; set; }
        public List<ApiEmployeeAttendanceDto> LastsEmployeeAttendances { get; set; } = new();
    }

    public sealed class ApiEmployeeAttendanceDto
    {
        public DateTime PunchTime { get; set; }
        public string PunchType { get; set; } = "";
        public string ShiftType { get; set; } = "";
    }
    public sealed class ApiUserRefreshDto
    {
        public string? LastPunchType { get; set; }
        public List<ApiEmployeeAttendanceDto>? LastsEmployeeAttendances { get; set; }
    }
}
