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
    private bool _isWithinTimeMargin = false;
    private bool _shiftCompleted = false;

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
    public bool IsActionButtonEnabled { get; private set; } = false;

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
        UpdateActionButtonState();
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
        Trace.WriteLine($"{_user.StartTime} == {_user.EndTime}");
        Trace.WriteLine($"{ToBrt(_user.StartTime)} == {ToBrt(_user.EndTime)}");

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

        // Verificar se o último registro é de hoje. Se não for, resetar o estado.
        if (last != null)
        {
            var lastBrt = ToBrt(last.PunchTime);
            var todayBrt = ToBrt(DateTime.UtcNow);
            if (lastBrt.Date != todayBrt.Date)
            {
                _isInNow = false;
                _shiftCompleted = false;
                _ring.ShiftCompleted = false;
                _ring.ShowProgress = false;
                _ring.StartMarkerColor = Colors.Green; // Resetar cor do marcador inicial
            }
        }

        ApplyHeader(_isInNow);

        LastPunchText = last is null
            ? string.Empty
            : ToBrt(last.PunchTime).ToString("'Às' HH:mm:ss", PtBr);

        RecentItems.Clear();
        if (_user.RecentActivities is not null)
            foreach (var a in _user.RecentActivities.OrderByDescending(x => x.PunchTime))
                RecentItems.Add(MapActivityToVM(a));

        UpdateRing();
        UpdateActionButtonState();
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

    private void UpdateActionButtonState()
    {
        if (_user is null || _shiftCompleted)
        {
            IsActionButtonEnabled = false;
            ActionButtonText = "Expediente Encerrado";
            ActionButtonBackground = Colors.Gray;
            return;
        }

        var now = ToBrt(DateTime.UtcNow);
        var start = ToBrt(_user.StartTime);
        var end = ToBrt(_user.EndTime);

        // Verificar se está dentro da margem de 10 minutos
        _isWithinTimeMargin = IsWithinTimeMargin(now, start, end);

        if (_isWithinTimeMargin)
        {
            IsActionButtonEnabled = true;
            ActionButtonText = _isInNow ? "Dar Saída" : "Dar Entrada";
            ActionButtonBackground = _isInNow ? Color.FromArgb("#962020") : Color.FromArgb("#4D9C24");
        }
        else
        {
            IsActionButtonEnabled = false;
            ActionButtonText = _isInNow ? "Aguardar Saída" : "Aguardar Expediente";
            ActionButtonBackground = Colors.Gray;
        }

        OnPropertyChanged(nameof(IsActionButtonEnabled));
        OnPropertyChanged(nameof(ActionButtonText));
        OnPropertyChanged(nameof(ActionButtonBackground));
    }

    private bool IsWithinTimeMargin(DateTime now, DateTime start, DateTime end)
    {
        var margin = TimeSpan.FromMinutes(10);

        // Para entrada: pode bater de 10min antes até o horário de início
        if (!_isInNow)
        {
            var timeToStart = start - now;
            return timeToStart <= margin && timeToStart >= TimeSpan.FromMinutes(-5); // 5min de tolerância após
        }
        // Para saída: pode bater de 10min antes até o horário de fim
        else
        {
            var timeToEnd = end - now;
            return timeToEnd <= margin && timeToEnd >= TimeSpan.FromMinutes(-5); // 5min de tolerância após
        }
    }

    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null || !IsActionButtonEnabled) return;

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

        LastPunchText = ToBrt(activity.PunchTime).ToString("'Às' HH:mm:ss", PtBr);

        if (_isInNow)
        {
            // Finalizar expediente
            _shiftCompleted = true;
            _ring.ShiftCompleted = true;
            _ring.ShowProgress = false;
            StopRingTimer(); // Parar o timer
        }
        else
        {
            // Iniciar expediente
            _isInNow = true;
            _ring.ShowProgress = true;
        }

        ApplyHeader(_isInNow);
        UpdateRing();
        UpdateActionButtonState();
        NotifyHeaderBindings();
    }

    private void ApplyHeader(bool isInNow)
    {
        StatusText = isInNow ? "Deu Entrada" : "Deu Saída";

        if (isInNow)
        {
            StatusColor = Color.FromArgb("#4D9C24");
            StatusFrameBackground = Color.FromArgb("#1A17BC08");
        }
        else
        {
            StatusColor = Color.FromArgb("#962020");
            StatusFrameBackground = Color.FromArgb("#1ABC1D08");
        }
    }

    private void UpdateRing(DateTime? nowBrtOpt = null)
    {
        if (_user is null) return;

        // INÍCIO
        var start = ToBrt(_user.StartTime);
        int startSec12 = ((start.Hour % 12) * 3600) + start.Minute * 60 + start.Second;
        _ring.StartMarkerAngleDeg = (float)(startSec12 / 43200.0 * 360.0);
        _ring.StartMarkerSweepDeg = 5f;

        // FIM
        var end = ToBrt(_user.EndTime);
        int endSec12 = ((end.Hour % 12) * 3600) + end.Minute * 60 + end.Second;
        _ring.EndMarkerAngleDeg = (float)(endSec12 / 43200.0 * 360.0);
        _ring.EndMarkerSweepDeg = 5f;

        // Ponteiro (hora atual 12h)
        var now = nowBrtOpt ?? ToBrt(DateTime.UtcNow);
        int nowSec12 = ((now.Hour % 12) * 3600) + now.Minute * 60 + now.Second;
        _ring.HandAngleDeg = (float)(nowSec12 / 43200.0 * 360.0);

        // CORREÇÃO: Ponteiro NUNCA fica cinza após dar entrada
        _ring.Dimmed = !_isWithinTimeMargin && !_isInNow && !_shiftCompleted;

        // Aplicar lógica de opacidade baseada na distância temporal
        ApplyMarkerOpacityLogic(start, end, now);

        ShiftRing.Invalidate();
    }

    private void ApplyMarkerOpacityLogic(DateTime start, DateTime end, DateTime now)
    {
        // Calcular distância em horas entre início e fim
        var timeSpanBetweenMarkers = end - start;
        if (timeSpanBetweenMarkers < TimeSpan.Zero)
        {
            timeSpanBetweenMarkers += TimeSpan.FromDays(1); // Caso passe da meia-noite
        }

        // Se a distância for maior que 12 horas, aplicar lógica de opacidade
        if (timeSpanBetweenMarkers > TimeSpan.FromHours(12))
        {
            // Calcular distância do horário atual para cada marcador
            var timeToStart = start - now;
            var timeToEnd = end - now;

            // Ajustar para considerar que pode ser no dia seguinte
            if (timeToStart < TimeSpan.Zero) timeToStart += TimeSpan.FromDays(1);
            if (timeToEnd < TimeSpan.Zero) timeToEnd += TimeSpan.FromDays(1);

            // Reduzir opacidade do marcador mais distante
            if (timeToStart > timeToEnd)
            {
                // Início está mais longe - reduzir opacidade do início
                _ring.StartMarkerColor = _ring.StartMarkerColor.WithAlpha(0.5f);
            }
            else
            {
                // Fim está mais longe - reduzir opacidade do fim
                _ring.EndMarkerColor = _ring.EndMarkerColor.WithAlpha(0.5f);
            }
        }
        else
        {
            // Resetar cores para opacidade total
            _ring.StartMarkerColor = Colors.Green;
            _ring.EndMarkerColor = Color.FromArgb("#962020");
        }
    }

    private void StartRingTimer()
    {
        if (_shiftCompleted) return; // Não iniciar timer se expediente completo

        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
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
        UpdateActionButtonState(); // Atualizar estado do botão a cada tick
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