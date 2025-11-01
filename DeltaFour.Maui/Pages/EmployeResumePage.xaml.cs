using DeltaFour.Maui.Local;
using System.Globalization;

namespace DeltaFour.Maui.Pages;
[QueryProperty(nameof(User), "user")]
public partial class EmployeResume : ContentPage
{
    private LocalUser? _user;
    public LocalUser? User
    {
        get => _user;
        set { _user = value; MapUserToView(); }
    }
    public string GreetingText { get; private set; } = "";
    public string StartTimeBrt { get; private set; } = "";
    public string EndTimeBrt { get; private set; } = "";
    public string ShiftTypeText { get; private set; } = "";
    public string StartedOnDate { get; private set; } = "";    
    public string WorkWindowText { get; private set; } = "";    
    public string CompanyNameText { get; private set; } = "";
    public string StatusText { get; private set; } = "";
    public Color StatusColor { get; private set; } = Colors.Transparent;
    public Color StatusFrameBackground { get; private set; } = Colors.Transparent;
    public string ActionButtonText { get; private set; } = "";
    public Color ActionButtonBackground { get; private set; } = Colors.Transparent;
    public string RecentPunchTypeText { get; private set; } = "";
    public string RecentPunchTimeBrt { get; private set; } = "";
    public string RecentShiftTypeText { get; private set; } = "";
    public string RecentDateShort { get; private set; } = "";
    public static readonly CultureInfo PtBr = new("pt-BR");
    public EmployeResume()
    {
        InitializeComponent();
        BindingContext = this;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        var now = DateTime.Now;
        var mesUpper = now.ToString("MMM", new CultureInfo("pt-BR"))
                          .ToUpper(new CultureInfo("pt-BR"));

        LblHoje.Text = $"Hoje {now:dd} {mesUpper} {now:yyyy}";
    }
    private void MapUserToView()
    {
        if (_user == null) return;

        var tz = GetBrt();
        var startBrt = TimeZoneInfo.ConvertTime(_user.StartTime, tz);
        var endBrt = TimeZoneInfo.ConvertTime(_user.EndTime, tz);

        GreetingText = $"Olá {_user.Name}!";
        StartTimeBrt = startBrt.ToString("HH:mm 'BRT'", PtBr);
        EndTimeBrt = endBrt.ToString("HH:mm 'BRT'", PtBr);
        ShiftTypeText = _user.ShiftType;
        StartedOnDate = startBrt.ToString("dd/MM/yy", PtBr);
        WorkWindowText = $"Das {startBrt:HH:mm} Até as {endBrt:HH:mm}";
        CompanyNameText = _user.CompanyName;

        // Última batida define o estado atual
        var last = _user.RecentActivities?
            .OrderByDescending(a => a.PunchTime)
            .FirstOrDefault();

        if (last != null)
        {
            var lastBrt = TimeZoneInfo.ConvertTime(last.PunchTime, tz);
            var isIn = string.Equals(last.PunchType, "IN", StringComparison.OrdinalIgnoreCase);

            StatusText = isIn ? "Deu Entrada" : "Deu Saída";

            if (isIn)
            {
                StatusColor = Color.FromArgb("#4D9C24");
                StatusFrameBackground = Color.FromArgb("#1A17BC08");

                ActionButtonText = "Dar Saída";
                ActionButtonBackground = Color.FromArgb("#962020");
            }
            else
            {
                StatusColor = Color.FromArgb("#962020");
                StatusFrameBackground = Color.FromArgb("#1ABC1D08");

                ActionButtonText = "Dar Entrada";
                ActionButtonBackground = Color.FromArgb("#4D9C24");
            }

            RecentPunchTypeText = isIn ? "Entrada" : "Saída";
            RecentPunchTimeBrt = lastBrt.ToString("HH:mm 'BRT'", PtBr);
            RecentShiftTypeText = last.ShiftType;
            RecentDateShort = lastBrt.ToString("MM/dd/yy", PtBr);
        }

        // Notifica o XAML
        OnPropertyChanged(nameof(GreetingText));
        OnPropertyChanged(nameof(StartTimeBrt));
        OnPropertyChanged(nameof(EndTimeBrt));
        OnPropertyChanged(nameof(ShiftTypeText));
        OnPropertyChanged(nameof(StartedOnDate));
        OnPropertyChanged(nameof(WorkWindowText));
        OnPropertyChanged(nameof(CompanyNameText));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(StatusFrameBackground));
        OnPropertyChanged(nameof(ActionButtonText));
        OnPropertyChanged(nameof(ActionButtonBackground));
        OnPropertyChanged(nameof(RecentPunchTypeText));
        OnPropertyChanged(nameof(RecentPunchTimeBrt));
        OnPropertyChanged(nameof(RecentShiftTypeText));
        OnPropertyChanged(nameof(RecentDateShort));
    }
    private static TimeZoneInfo GetBrt()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); }        
    }
    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null) return;

        var isIn = StatusText.Equals("Deu Entrada", StringComparison.OrdinalIgnoreCase);
        var tz = GetBrt();
        var nowBrt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        _user.RecentActivities ??= new();
        _user.RecentActivities.Add(new RecentActivity
        {
            PunchTime = nowBrt,
            PunchType = isIn ? "OUT" : "IN",
            ShiftType = _user.ShiftType
        });

        MapUserToView();
    }

}