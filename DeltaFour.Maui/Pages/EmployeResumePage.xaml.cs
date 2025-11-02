using DeltaFour.Maui.Local;
using DeltaFour.Maui.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DeltaFour.Maui.Pages;

public partial class EmployeResume : ContentPage
{
    private ISession? session;
    private LocalUser? _user;

    public ObservableCollection<RecentItemVM> RecentItems { get; } = new();

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
        HandlerChanged += OnHandlerChanged;
    }

    private bool TryResolveSession()
    {
        session ??= Handler?.MauiContext?.Services.GetService<ISession>()
               ?? Application.Current?.Handler?.MauiContext?.Services.GetService<ISession>();
        return session is not null;
    }

    private void HydrateFromSession()
    {
        if (session?.CurrentUser is LocalUser u && !ReferenceEquals(_user, u))
        {
            _user = u;
            MapUserToView();     
        }
    }

    private void OnHandlerChanged(object? s, EventArgs e)
    {
        if (TryResolveSession()) HydrateFromSession();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var now = DateTime.Now;
        var mesUpper = now.ToString("MMM", PtBr).ToUpper(PtBr);
        LblHoje.Text = $"Hoje {now:dd} {mesUpper} {now:yyyy}";
    }

    private void MapUserToView()
    {
        if (_user == null) return;

        var tz = GetBrt();
        var startBrt = TimeZoneInfo.ConvertTime(_user.StartTime, tz);
        var endBrt = TimeZoneInfo.ConvertTime(_user.EndTime, tz);

        GreetingText = $"{_user.Name}";
        StartTimeBrt = startBrt.ToString("HH:mm 'BRT'", PtBr);
        EndTimeBrt = endBrt.ToString("HH:mm 'BRT'", PtBr);
        ShiftTypeText = _user.ShiftType;
        StartedOnDate = startBrt.ToString("dd/MM/yy", PtBr);
        WorkWindowText = $"Das {startBrt:HH:mm} Até as {endBrt:HH:mm}";
        CompanyNameText = _user.CompanyName;

        UpdateHeaderFromLast();

        RecentItems.Clear();
        if (_user.RecentActivities is not null)
        {
            foreach (var a in _user.RecentActivities.OrderByDescending(x => x.PunchTime))
                RecentItems.Add(MapActivityToVM(a, tz));
        }

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

    private RecentItemVM MapActivityToVM(RecentActivity a, TimeZoneInfo tz)
    {
        var t = TimeZoneInfo.ConvertTime(a.PunchTime, tz);
        var isIn = string.Equals(a.PunchType, "IN", StringComparison.OrdinalIgnoreCase);
        return new RecentItemVM
        {
            PunchTypeText = isIn ? "Entrada" : "Saída",
            PunchTimeBrt = t.ToString("HH:mm 'BRT'", PtBr),
            ShiftTypeText = a.ShiftType,
            DateShort = t.ToString("MM/dd/yy", PtBr)
        };
    }

    private void UpdateHeaderFromLast()
    {
        if (_user?.RecentActivities is null || _user.RecentActivities.Count == 0) return;

        var tz = GetBrt();
        var last = _user.RecentActivities.OrderByDescending(a => a.PunchTime).First();
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

    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null) return;

        var isInNow = StatusText.Equals("Deu Entrada", StringComparison.OrdinalIgnoreCase);

        var tz = GetBrt();
        var nowBrt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        _user.RecentActivities ??= new();
        var newActivity = new RecentActivity
        {
            PunchTime = nowBrt,                  
            PunchType = isInNow ? "OUT" : "IN",
            ShiftType = _user.ShiftType
        };
        _user.RecentActivities.Add(newActivity);

        RecentItems.Insert(0, MapActivityToVM(newActivity, tz));

        UpdateHeaderFromLast();
    }

    public sealed class RecentItemVM
    {
        public string PunchTypeText { get; set; } = "";
        public string PunchTimeBrt { get; set; } = "";
        public string ShiftTypeText { get; set; } = "";
        public string DateShort { get; set; } = "";
    }
}
