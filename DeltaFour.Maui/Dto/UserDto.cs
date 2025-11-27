using System;
using System.Collections.Generic;

namespace DeltaFour.Maui.Dto
{
    public sealed class ApiUserDto
    {
        public string Name { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime StartDate { get; set; }
        public string ShiftType { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public string? LastPunchType { get; set; }

        public List<ApiEmployeeAttendanceDto> LastUserAttendances { get; set; } = new();
    }

    public sealed class ApiEmployeeAttendanceDto
    {
        public DateTime PunchTime { get; set; }
        public DateTime PunchDate { get; set; } 
        public string PunchType { get; set; } = "";
        public string ShiftType { get; set; } = "";
    }
}
