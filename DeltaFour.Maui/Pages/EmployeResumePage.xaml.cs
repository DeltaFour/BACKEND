using DeltaFour.Maui.Local;
using DeltaFour.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace DeltaFour.Maui.Pages;

public partial class EmployeResume : ContentPage
{
    private ISession? session;
    private LocalUser? _user;

    private bool _isInNow;

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

    private void OnHandlerChanged(object? s, EventArgs e)
    {
        if (TryResolveSession()) LoadUserIfChanged();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (TryResolveSession()) LoadUserIfChanged();

        var nowBrt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GetBrt());
        var mesUpper = nowBrt.ToString("MMM", PtBr).ToUpper(PtBr);
        LblHoje.Text = $"Hoje - {nowBrt:dd} {mesUpper} {nowBrt:yyyy}";
    }

    async void OnLogoutClicked(object? sender, EventArgs e)
    {
        if (!TryResolveSession())
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            return;
        }

        session!.IsAuthenticated = false;
        session.CurrentUser = null;

        _user = null;
        RecentItems.Clear();

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
    private void LoadUserIfChanged()
    {
        if (session?.CurrentUser is not LocalUser u || ReferenceEquals(_user, u))
            return;

        _user = u;
        _user.StartTime = ToUtcAssumingBrt(_user.StartTime);
        _user.EndTime = ToUtcAssumingBrt(_user.EndTime);

        if (_user.RecentActivities is not null)
        {
            for (int i = 0; i < _user.RecentActivities.Count; i++)
            {
                var a = _user.RecentActivities[i];
                a.PunchTime = ToUtcAssumingBrt(a.PunchTime);
            }
        }

        FillAllFromUser();
    }

    private void FillAllFromUser()
    {
        if (_user is null) return;

        var startBrt = ToBrt(_user.StartTime);
        var endBrt = ToBrt(_user.EndTime);

        GreetingText = $"{_user.Name}";
        StartTimeBrt = startBrt.ToString("HH:mm 'BRT'", PtBr);
        EndTimeBrt = endBrt.ToString("HH:mm 'BRT'", PtBr);
        ShiftTypeText = _user.ShiftType;
        StartedOnDate = startBrt.ToString("dd/MM/yy", PtBr);
        WorkWindowText = $"Das {startBrt:HH:mm} Até as {endBrt:HH:mm}";
        CompanyNameText = _user.CompanyName;

        var last = _user.RecentActivities?
            .OrderByDescending(a => a.PunchTime)
            .FirstOrDefault();

        if (last is null)
        {
            _isInNow = false; 
            SetHeader(isInNow: _isInNow, tBrt: null, shiftType: _user.ShiftType, hasRecent: false);
        }
        else
        {
            _isInNow = last.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
            var lastBrt = ToBrt(last.PunchTime);
            SetHeader(isInNow: _isInNow, tBrt: lastBrt, shiftType: last.ShiftType, hasRecent: true);
        }
        RecentItems.Clear();
        if (_user.RecentActivities is not null)
        {
            foreach (var a in _user.RecentActivities.OrderByDescending(x => x.PunchTime))
                RecentItems.Add(MapActivityToVM(a));
        }
        NotifyHeaderBindings();
    }
    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null)
        {
            Debug.WriteLine("Warn: _user null no clique.");
            return;
        }
        var newType = _isInNow ? "OUT" : "IN";
        var utcNow = DateTime.UtcNow;

        var activity = new RecentActivity
        {
            PunchTime = utcNow, 
            PunchType = newType,
            ShiftType = _user.ShiftType
        };

        _user.RecentActivities ??= new();
        _user.RecentActivities.Add(activity);

        RecentItems.Insert(0, MapActivityToVM(activity));

        _isInNow = !_isInNow;
        var tBrt = ToBrt(activity.PunchTime);
        SetHeader(_isInNow, tBrt, activity.ShiftType, hasRecent: true);
        NotifyHeaderBindings();
    }
    private void SetHeader(bool isInNow, DateTime? tBrt, string shiftType, bool hasRecent)
    {
        StatusText = isInNow ? "Deu Entrada" : "Deu Saída";

        if (isInNow)
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

        if (hasRecent && tBrt is DateTime dt)
        {
            RecentPunchTypeText = isInNow ? "Entrada" : "Saída";
            RecentPunchTimeBrt = dt.ToString("HH:mm 'BRT'", PtBr);
            RecentShiftTypeText = shiftType;
            RecentDateShort = dt.ToString("dd/MM/yy", PtBr);
        }
        else
        {
            RecentPunchTypeText = "";
            RecentPunchTimeBrt = "";
            RecentShiftTypeText = "";
            RecentDateShort = "";
        }
    }
    private void NotifyHeaderBindings()
    {
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
    private RecentItemVM MapActivityToVM(RecentActivity a)
    {
        var t = ToBrt(a.PunchTime);
        var in_ = a.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
        return new RecentItemVM
        {
            PunchTypeText = in_ ? "Entrada" : "Saída",
            PunchTimeBrt = t.ToString("HH:mm 'BRT'", PtBr),
            ShiftTypeText = a.ShiftType,
            DateShort = t.ToString("dd/MM/yy", PtBr)
        };
    }
    private static TimeZoneInfo GetBrt()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); }
    }
    private static DateTime ToBrt(DateTime dt)
    {
        var tz = GetBrt();
        return dt.Kind switch
        {
            DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(dt, tz),
            DateTimeKind.Local => TimeZoneInfo.ConvertTime(dt, tz),
            DateTimeKind.Unspecified => TimeZoneInfo.ConvertTime(dt, tz, tz),
            _ => TimeZoneInfo.ConvertTime(dt, tz)
        };
    }
    private static DateTime ToUtcAssumingBrt(DateTime dt)
    {
        var tz = GetBrt();
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeToUtc(dt, tz),
            _ => dt
        };
    }
    public sealed class RecentItemVM
    {
        public string PunchTypeText { get; set; } = "";
        public string PunchTimeBrt { get; set; } = "";
        public string ShiftTypeText { get; set; } = "";
        public string DateShort { get; set; } = "";
    }
}
