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

    // LastPunchText como BindableProperty para atualização imediata no XAML
    public static readonly BindableProperty LastPunchTextProperty =
        BindableProperty.Create(nameof(LastPunchText), typeof(string), typeof(EmployeResume), default(string));
    public string LastPunchText
    {
        get => (string)GetValue(LastPunchTextProperty);
        private set => SetValue(LastPunchTextProperty, value);
    }

    private readonly ShiftRingDrawable _ring = new();
    private IDispatcherTimer? _timer;

    public static readonly CultureInfo PtBr = new("pt-BR");

    public EmployeResume()
    {
        InitializeComponent();
        BindingContext = this;
        HandlerChanged += OnHandlerChanged;
        ShiftRing.Drawable = _ring;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (TryResolveSession()) LoadUserIfChanged();
        StartRingTimer();
        UpdateRing();
        UpdateLblHoje();
    }

    protected override void OnDisappearing()
    {
        StopRingTimer();
        base.OnDisappearing();
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

        GreetingText = _user.Name;
        StartTimeBrt = startBrt.ToString("HH:mm 'BRT'", PtBr);
        EndTimeBrt = endBrt.ToString("HH:mm 'BRT'", PtBr);
        ShiftTypeText = _user.ShiftType;
        StartedOnDate = startBrt.ToString("dd/MM/yy", PtBr);
        WorkWindowText = $"Das {startBrt:HH:mm} Até as {endBrt:HH:mm}";
        CompanyNameText = _user.CompanyName;

        var last = _user.RecentActivities?.OrderByDescending(a => a.PunchTime).FirstOrDefault();
        _isInNow = last?.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase) == true;

        ApplyHeader(_isInNow);

        LastPunchText = last is null
            ? string.Empty
            : ToBrt(last.PunchTime).ToString("'Às' HH:mm:ss", PtBr);

        RecentItems.Clear();
        if (_user.RecentActivities is not null)
            foreach (var a in _user.RecentActivities.OrderByDescending(x => x.PunchTime))
                RecentItems.Add(MapActivityToVM(a));

        UpdateRing();
        NotifyHeaderBindings();
    }

    private void UpdateLblHoje()
    {
        var nowBrt = ToBrt(DateTime.UtcNow);
        var mesUpper = nowBrt.ToString("MMM", PtBr).ToUpper(PtBr);

        Color primary = Colors.Black;
        if (Application.Current.Resources.TryGetValue("Primary", out var v))
            primary = v is SolidColorBrush b ? b.Color : (Color)v;

        LblHoje.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = "Hoje - " },
                new Span { Text = nowBrt.ToString("dd ", PtBr), FontAttributes = FontAttributes.Bold, TextColor = primary },
                new Span { Text = mesUpper + " " },
                new Span { Text = nowBrt.ToString("yyyy", PtBr), FontAttributes = FontAttributes.Bold, TextColor = primary },
            }
        };
    }

    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null) return;

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

        // atualiza dica de horário imediatamente
        LastPunchText = ToBrt(activity.PunchTime).ToString("'Às' HH:mm:ss", PtBr);

        _isInNow = !_isInNow;
        ApplyHeader(_isInNow);

        UpdateRing();           // mantém ponteiro/progresso corretos
        NotifyHeaderBindings();
    }

    private void ApplyHeader(bool isInNow)
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
    }

    // ---------- RING ----------

    private static float DayPos(DateTime brt)
        => (float)(brt.TimeOfDay.TotalSeconds / 86400d);

    private void UpdateRing(DateTime? nowBrtOpt = null)
    {
        if (_user is null) return;

        var nowBrt = ToBrt(DateTime.UtcNow);
        NowBrtLabel.Text = nowBrt.ToString("HH:mm:ss", PtBr);

        // Ângulo do ponteiro (relógio de 12h)
        var seconds12 = (nowBrt.Hour % 12) * 3600 + nowBrt.Minute * 60 + nowBrt.Second;
        _ring.HandAngleDeg = (float)(seconds12 / 43200.0 * 360.0);

        // Ângulo do INÍCIO do expediente (relógio de 12h): 30° por hora + 0.5° por minuto + 1/120° por segundo
        var startBrt = ToBrt(_user.StartTime);
        float startAngleDeg =
            (startBrt.Hour % 12) * 30f
            + startBrt.Minute * 0.5f
            + startBrt.Second * (0.5f / 60f);

        _ring.StartMarkerAngleDeg = startAngleDeg;
        _ring.StartMarkerSweepDeg = 5f;      // 10 minutos na escala de 12h
        _ring.StartMarkerColor = Colors.Green;

        _ring.Dimmed = !_isInNow;            // fora de expediente: dim
        ShiftRing.Invalidate();

    }

    private void StartRingTimer()
    {
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);

        // garante um único handler
        _timer.Tick -= OnTimerTick;
        _timer.Tick += OnTimerTick;

        if (!_timer.IsRunning)
            _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var nowBrt = ToBrt(DateTime.UtcNow);
        NowBrtLabel.Text = nowBrt.ToString("HH:mm:ss", PtBr);
        UpdateRing(nowBrt);
    }

    private void StopRingTimer()
    {
        if (_timer is null) return;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }

    // ---------- Bindings/util ----------

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
    }

    private RecentItemVM MapActivityToVM(RecentActivity a)
    {
        var t = ToBrt(a.PunchTime);
        var isIn = a.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
        return new RecentItemVM
        {
            PunchTypeText = isIn ? "Entrada" : "Saída",
            PunchTimeBrt = t.ToString("HH:mm 'BRT'", PtBr),
            ShiftTypeText = a.ShiftType,
            DateShort = t.ToString("dd/MM/yy", PtBr)
        };
    }

    // ——— fuso util ———
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
