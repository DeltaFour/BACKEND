namespace DeltaFour.Maui.Local
{
    public class LocalUser
    {
        public string Name { get; set; } = "";
        public string ShiftType { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CompanyName { get; set; } = "";

        public int ToleranceMinutes { get; set; } = 10;

        public List<RecentActivity> RecentActivities { get; set; } = new();
    }



    public class RecentActivity
    {
        public DateTime PunchTime { get; set; }
        public string PunchType { get; set; } = "";
        public string ShiftType { get; set; } = "";
    }

}
