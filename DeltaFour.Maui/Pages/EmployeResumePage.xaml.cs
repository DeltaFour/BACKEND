#if ANDROID
using Android.Content;
using Android.Net;
#endif
using DeltaFour.Maui.Local;
using DeltaFour.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace DeltaFour.Maui.Pages;
public partial class EmployeResume : ContentPage
{
    private ISession? session;
    private IApiService apiService;
    private LocalUser? _user;
    private bool _firstAppearing = true;
    private bool _isInNow;
    private bool _shiftCompleted;
    private DateTime? _lastOutBrt;
    private bool _hasLastPunch;
    private bool _lastWasIn;
    private bool _lastWasOut;
    private DateTime? _entryBrt;
    private DateTime _currentNowBrt;
    private int _timeMultiplier = 1;
    public ObservableCollection<RecentItemVM> RecentItems { get; } = new();
    public string GreetingText { get; private set; } = "";
    public string StartTimeBrt { get; private set; } = "";
    public string EndTimeBrt { get; private set; } = "";
    public string ShiftTypeText { get; private set; } = "";
    public string GreetingPrefix { get; private set; } = "Olá ";
    public string StartedOnDate { get; private set; } = "";
    public string WorkWindowText { get; private set; } = "";
    public string CompanyNameText { get; private set; } = "";
    public string StatusText { get; private set; } = "";
    public Color StatusColor { get; private set; } = Colors.Transparent;
    public Color StatusFrameBackground { get; private set; } = Colors.Transparent;
    public string ActionButtonText { get; private set; } = "";
    public Color ActionButtonBackground { get; private set; } = Colors.Transparent;
    public bool IsActionButtonEnabled { get; private set; }
    public static readonly BindableProperty LastPunchTextProperty =
        BindableProperty.Create(nameof(LastPunchText), typeof(string), typeof(EmployeResume), default(string));

    public string LastPunchText
    {
        get => (string)GetValue(LastPunchTextProperty);
        private set => SetValue(LastPunchTextProperty, value);
    }

    public static readonly BindableProperty LastPunchFormattedProperty =
        BindableProperty.Create(nameof(LastPunchFormatted), typeof(FormattedString), typeof(EmployeResume), default(FormattedString));

    public FormattedString LastPunchFormatted
    {
        get => (FormattedString)GetValue(LastPunchFormattedProperty);
        private set => SetValue(LastPunchFormattedProperty, value);
    }
    private readonly ShiftRingDrawable _ring = new();
    private IDispatcherTimer? _timer;
    public static readonly CultureInfo PtBr = new("pt-BR");

    /// <summary>
    /// Inicializa a página e configura bindings e drawable do relógio.
    /// </summary>
    public EmployeResume()
    {
        InitializeComponent();
        BindingContext = this;
        HandlerChanged += OnHandlerChanged;
        ShiftRing.Drawable = _ring;
    }

    #region Helpers de “now” (DEBUG vs normal)

    /// <summary>
    /// Indica se o relógio em modo demo deve ser usado.
    /// </summary>
    private bool UseDemoClock
    {
        get
        {
#if DEBUG
            return session?.IsDemoTime == true;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// Abre o cliente de e-mail para envio de feedback.
    /// </summary>
    private async void OnEnviarFeedbackClicked(object? sender, EventArgs e)
    {
#if ANDROID
        try
        {
            var message = new EmailMessage
            {
                Subject = "Feedback do app",
                Body = "",
                To = new List<string> { "arthur.calchi@gmail.com" }
            };
            await Email.Default.ComposeAsync(message);
            return;
        }
        catch (FeatureNotSupportedException)
        {
            try
            {
                var uri = Android.Net.Uri.Parse("mailto:arthur.calchi@gmail.com?subject=Feedback%20do%20app");
                var intent = new Android.Content.Intent(Intent.ActionSendto, uri);
                var activity = Platform.CurrentActivity;
                activity?.StartActivity(intent);
                return;
            }
            catch
            {
                await DisplayAlert("Erro", "Não foi possível abrir o aplicativo de e-mail.", "OK");
            }
        }
#else
        try
        {
            var message = new EmailMessage
            {
                Subject = "Feedback do app",
                Body = "",
                To = new List<string> { "arthur.calchi@gmail.com" }
            };
            await Email.Default.ComposeAsync(message);
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Não foi possível abrir o aplicativo de e-mail.", "OK");
        }
#endif
    }

    /// <summary>
    /// Retorna o horário atual em BRT, avançando tempo em modo demo.
    /// </summary>
    /// <returns>Instante atual em BRT.</returns>
    private DateTime GetNowBrt()
    {
        if (UseDemoClock && session is not null)
        {
            if (session.DemoNowBrt is null)
            {
                session.DemoNowBrt = ToBrt(DateTime.UtcNow);
            }
            else
            {
                session.DemoNowBrt = session.DemoNowBrt.Value.AddSeconds(_timeMultiplier);
            }
            _currentNowBrt = session.DemoNowBrt.Value;
            return _currentNowBrt;
        }
        _currentNowBrt = ToBrt(DateTime.UtcNow);
        return _currentNowBrt;
    }

    /// <summary>
    /// Retorna um snapshot do horário atual em BRT sem avançar o tempo.
    /// </summary>
    /// <returns>Instante atual em BRT sem alterar o relógio demo.</returns>
    private DateTime GetSnapshotNowBrt()
    {
        if (UseDemoClock && session?.DemoNowBrt is not null)
            return session.DemoNowBrt.Value;
        return ToBrt(DateTime.UtcNow);
    }

    #endregion

    /// <summary>
    /// Efetua logout, limpa sessão e navega para a tela de login.
    /// </summary>
    async void OnLogoutClicked(object? sender, EventArgs e)
    {
        var app = (App)Application.Current;
        app.MainPage = new ContentPage
        {
            BackgroundColor = Color.FromArgb("#F5F8FB"),
            Content = new Grid
            {
                Children =
            {
                new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Saindo...",
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0,0,0,40)
                }
            }
            }
        };
        if (!TryResolveDependencies())
        {
            await app.ExitToLoginAsync();
            return;
        }
        try
        {
            await apiService!.LogoutAsync();
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Erro no logout da API: {ex}");
        }
        await app.ExitToLoginAsync();
    }


    /// <summary>
    /// Resolve ISession e IApiService a partir do container de DI.
    /// </summary>
    /// <returns>True se as dependências foram resolvidas.</returns>
    private bool TryResolveDependencies()
    {
        if (session != null && apiService != null)
            return true;
        var services = Handler?.MauiContext?.Services ?? Application.Current?.Handler?.MauiContext?.Services;
        if (services == null)
            return false;
        session ??= services.GetRequiredService<ISession>();
        apiService ??= services.GetRequiredService<IApiService>();
        return true;
    }

    /// <summary>
    /// Intercepta o botão de voltar no Android e pergunta se o usuário deseja sair.
    /// </summary>
    /// <returns>True se o back foi tratado.</returns>
    protected override bool OnBackButtonPressed()
    {
#if ANDROID
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool sair = await DisplayAlert("Sair", "Deseja sair?", "Sim", "Não");
            if (sair)
            {
                OnLogoutClicked(this, EventArgs.Empty);
            }
        });
        return true;
#else
        return base.OnBackButtonPressed();
#endif
    }

    /// <summary>
    /// Atualiza o prefixo de saudação com base no horário atual.
    /// </summary>
    private void UpdateGreetingPrefix(DateTime nowBrt)
    {
        var baseGreetings = new[]
        {
            "Olá, ",
            "Bem-vindo, "
        };
        var candidates = new List<string>(baseGreetings);
        var hour = nowBrt.Hour;
        if (hour >= 5 && hour < 12)
        {
            candidates.Add("Bom dia, ");
        }
        else if (hour >= 12 && hour < 18)
        {
            candidates.Add("Boa tarde, ");
        }
        else
        {
            candidates.Add("Boa noite, ");
        }
        var index = Random.Shared.Next(candidates.Count);
        GreetingPrefix = candidates[index];
        OnPropertyChanged(nameof(GreetingPrefix));
    }

    /// <summary>
    /// Atualiza o usuário a partir da API e refaz o preenchimento da tela.
    /// </summary>
    private async Task RefreshUserFromApiAsync()
    {
        if (!TryResolveDependencies())
            return;
        if (session?.CurrentUser is not LocalUser user)
            return;
        try
        {
            var updated = await apiService!.RefreshUserAsync(user);
            if (updated is null)
                return;
            session.CurrentUser = updated;
            var nowBrt = GetSnapshotNowBrt();
            _currentNowBrt = nowBrt;
            FillAllFromUser(nowBrt);
        }
        catch (ApiUnavailableException)
        {
            var retry = await DisplayAlert("Aviso", "Servidor indisponível para atualizar suas informações de ponto.", "Tentar novamente", "");
            if (retry)
                await RefreshUserFromApiAsync();
        }
        catch (ApiException ex)
        {
            Trace.WriteLine($"[EmployeResume] Erro ao atualizar usuário: {ex}");
        }
    }

    /// <summary>
    /// Ciclo de vida: ao aparecer, atualiza relógio, UI e tenta refresh do usuário.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (TryResolveDependencies())
            LoadUserIfChanged();
        var nowBrt = GetNowBrt();
        UpdateGreetingPrefix(nowBrt);
        UpdateRing(nowBrt);
        UpdateActionButtonState(nowBrt);
        UpdateLblHoje();
        StartRingTimer();
        if (_firstAppearing)
        {
            _firstAppearing = false;
            return;
        }
        _ = RefreshUserFromApiAsync();
    }

    /// <summary>
    /// Ciclo de vida: ao desaparecer, para o timer do relógio.
    /// </summary>
    protected override void OnDisappearing()
    {
        StopRingTimer();
        base.OnDisappearing();
    }

    /// <summary>
    /// Reage à mudança de Handler e recarrega o usuário se necessário.
    /// </summary>
    private void OnHandlerChanged(object? s, EventArgs e)
    {
        if (TryResolveDependencies())
            LoadUserIfChanged();
    }

    /// <summary>
    /// Carrega o usuário da sessão se a referência mudou e normaliza horários.
    /// </summary>
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
        var nowBrt = _currentNowBrt != default ? _currentNowBrt : GetSnapshotNowBrt();
        FillAllFromUser(nowBrt);
    }

    /// <summary>
    /// Extrai o primeiro nome a partir do nome completo.
    /// </summary>
    /// <returns>Primeiro nome ou string vazia.</returns>
    private static string GetFirstName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : fullName.Trim();
    }

    /// <summary>
    /// Preenche todo o estado da tela com base no usuário e horário atual em BRT.
    /// </summary>
    private void FillAllFromUser(DateTime nowBrt)
    {
        if (_user is null)
            return;
        var startTemplate = ToBrt(_user.StartTime);
        var endTemplate = ToBrt(_user.EndTime);
        var rawStart = startTemplate;
        var rawEnd = endTemplate;
        GetShiftWindowForNow(nowBrt, out var shiftStartToday, out var shiftEndToday);
        var tol = GetTolerance();
        var windowStart = shiftStartToday - tol;
        var lateLimit = shiftEndToday + tol + TimeSpan.FromHours(12);
        GreetingText = GetFirstName(_user.Name);
        StartTimeBrt = startTemplate.ToString("HH:mm 'BRT'", PtBr);
        EndTimeBrt = endTemplate.ToString("HH:mm 'BRT'", PtBr);
        ShiftTypeText = _user.ShiftType;
        StartedOnDate = startTemplate.ToString("dd/MM/yy", PtBr);
        WorkWindowText = $"Das {startTemplate:HH:mm} Até as {endTemplate:HH:mm}";
        CompanyNameText = _user.CompanyName;
        _isInNow = false;
        _shiftCompleted = false;
        _entryBrt = null;
        _lastOutBrt = null;
        _hasLastPunch = false;
        _lastWasIn = false;
        _lastWasOut = false;
        LastPunchText = string.Empty;
        LastPunchFormatted = string.Empty;
        if (_user.RecentActivities is null || _user.RecentActivities.Count == 0)
        {
            RecentItems.Clear();
            ApplyHeader();
            UpdateRing(nowBrt);
            UpdateActionButtonState(nowBrt);
            NotifyHeaderBindings();
            return;
        }
        var actsWithBrt = _user.RecentActivities
            .Select(a => new { Act = a, TimeBrt = ToBrt(a.PunchTime) })
            .OrderBy(x => x.TimeBrt)
            .ToList();
        var lastOverall = actsWithBrt.Last();
        _hasLastPunch = true;
        _lastWasIn = lastOverall.Act.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
        _lastWasOut = lastOverall.Act.PunchType.Equals("OUT", StringComparison.OrdinalIgnoreCase);
        var lastTime = lastOverall.TimeBrt;
        LastPunchText = lastTime.ToString("'Às' HH:mm:ss", PtBr);

        var fsLast = new FormattedString();
        fsLast.Spans.Add(new Span { Text = "Às ",
            TextColor = Color.FromArgb("#8EA0B3")        
        });
        fsLast.Spans.Add(new Span
        {
            TextColor = Color.FromArgb("#8EA0B3"),
            Text = lastTime.ToString("HH:mm:ss", PtBr),
            FontAttributes = FontAttributes.Bold
        });
        LastPunchFormatted = fsLast; var todaysActs = actsWithBrt
            .Where(x => x.TimeBrt >= windowStart && x.TimeBrt <= lateLimit)
            .ToList();
        if (todaysActs.Count > 0)
        {
            var lastToday = todaysActs.Last();
            var lastTodayIsIn = lastToday.Act.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
            var lastTodayIsOut = lastToday.Act.PunchType.Equals("OUT", StringComparison.OrdinalIgnoreCase);
            var lastInToday = todaysActs
                .Where(x => x.Act.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase))
                .LastOrDefault();
            if (lastTodayIsOut && lastInToday is not null)
            {
                _isInNow = false;
                _shiftCompleted = true;
                _entryBrt = lastInToday.TimeBrt;
                _lastOutBrt = lastToday.TimeBrt;
            }
            else if (lastTodayIsIn)
            {
                _isInNow = true;
                _shiftCompleted = false;
                _entryBrt = lastToday.TimeBrt;
                _lastOutBrt = null;
            }
        }
        RecentItems.Clear();
        foreach (var x in actsWithBrt.OrderByDescending(x => x.TimeBrt))
            RecentItems.Add(MapActivityToVM(x.Act));
        ApplyHeader();
        UpdateRing(nowBrt);
        UpdateActionButtonState(nowBrt);
        NotifyHeaderBindings();
    }

    /// <summary>
    /// Atualiza o label de "Hoje" com a data atual em BRT.
    /// </summary>
    private void UpdateLblHoje()
    {
        var nowBrt = GetSnapshotNowBrt();
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
                new Span { Text = nowBrt.ToString("yyyy", PtBr), FontAttributes = FontAttributes.Bold, TextColor = primary }
            }
        };
    }

    /// <summary>
    /// Atualiza o estado, texto e cor do botão principal de ação.
    /// </summary>
    private void UpdateActionButtonState(DateTime nowBrt)
    {
        if (_user is null)
        {
            IsActionButtonEnabled = false;
            ActionButtonText = "Aguardar Expediente";
            ActionButtonBackground = Color.FromArgb("#a1a1a1");
            NotifyActionButtonBindings();
            return;
        }
        var tol = GetTolerance();
        var rawStart = ToBrt(_user.StartTime);
        var rawEnd = ToBrt(_user.EndTime);
        DateTime shiftStart;
        DateTime shiftEnd;
        if (_isInNow && _entryBrt.HasValue)
        {
            shiftStart = GetShiftStartForHit(rawStart, rawEnd, _entryBrt.Value);
            shiftEnd = GetShiftEndForHit(rawStart, rawEnd, _entryBrt.Value);
        }
        else
        {
            GetShiftWindowForNow(nowBrt, out shiftStart, out shiftEnd);
        }
        var entryWinStart = shiftStart - tol;
        var entryWinEnd = shiftStart + tol;
        var exitWinStart = shiftEnd - tol;
        var exitWinEnd = shiftEnd + tol;
        bool inEntryWindow = nowBrt >= entryWinStart && nowBrt <= entryWinEnd;
        bool afterEntryWin = nowBrt > entryWinEnd;
        bool beforeEntryWin = nowBrt < entryWinStart;
        bool inExitWindow = nowBrt >= exitWinStart && nowBrt <= exitWinEnd;
        bool afterExitWin = nowBrt > exitWinEnd;
        bool beforeExitWin = nowBrt < exitWinStart;
        if (_shiftCompleted)
        {
            IsActionButtonEnabled = false;
            ActionButtonText = "Expediente Encerrado";
            ActionButtonBackground = Color.FromArgb("#a1a1a1");
        }
        else if (!_isInNow)
        {
            if (beforeEntryWin)
            {
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Expediente";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
            else if (inEntryWindow)
            {
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Entrada";
                ActionButtonBackground = Color.FromArgb("#4D9C24");
            }
            else if (afterEntryWin && nowBrt <= exitWinEnd)
            {
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Entrada (Atraso)";
                ActionButtonBackground = Color.FromArgb("#4D9C24");
            }
            else
            {
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Expediente";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
        }
        else
        {
            if (beforeExitWin)
            {
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Saída";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
            else if (inExitWindow)
            {
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Saída";
                ActionButtonBackground = Color.FromArgb("#962020");
            }
            else if (afterExitWin && nowBrt <= shiftEnd + tol + TimeSpan.FromHours(12))
            {
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Saída (Tardia)";
                ActionButtonBackground = Color.FromArgb("#962020");
            }
            else
            {
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Expediente";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
        }
        NotifyActionButtonBindings();
    }

    /// <summary>
    /// Manipula o clique do botão principal e navega para a página de reconhecimento facial.
    /// </summary>
    private async void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_user == null || !IsActionButtonEnabled)
            return;
        if (!TryResolveDependencies())
            return;
        var nowBrt = GetNowBrt();
        var punchingOut = _isInNow;
        var newType = punchingOut ? "OUT" : "IN";
        bool canPunch;
        try
        {
            canPunch = await apiService!.CanPunchAsync(nowBrt, punchingOut);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Erro ao validar ponto: {ex}");
            await DisplayAlert("Erro", "Não foi possível validar o registro de ponto.", "OK");
            return;
        }
        if (!canPunch)
        {
            await DisplayAlert("Aviso", "No momento não é permitido registrar o ponto.", "OK");
            return;
        }
        var typeParam = System.Uri.EscapeDataString(newType);
        var timeParam = System.Uri.EscapeDataString(nowBrt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture));
        var route = $"//MainTabs/FaceRegisterPage?punchType={typeParam}&timeBrt={timeParam}";
        await Shell.Current.GoToAsync(route);
    }

    /// <summary>
    /// Acelera o relógio quando o botão principal é pressionado.
    /// </summary>
    private void OnActionButtonPressed(object? sender, EventArgs e)
    {
        _timeMultiplier = 1040;
    }

    /// <summary>
    /// Restaura a velocidade normal do relógio ao soltar o botão.
    /// </summary>
    private void OnActionButtonReleased(object? sender, EventArgs e)
    {
        _timeMultiplier = 1;
    }

    /// <summary>
    /// Atualiza o cabeçalho de status com base na última batida.
    /// </summary>
    private void ApplyHeader()
    {
        if (!_hasLastPunch)
        {
            StatusText = "Sem Registro";
            StatusColor = Color.FromArgb("#A1A1A1");
            StatusFrameBackground = Color.FromArgb("#1A989898");
            return;
        }
        if (_lastWasOut)
        {
            StatusText = "Deu Saída";
            StatusColor = Color.FromArgb("#962020");
            StatusFrameBackground = Color.FromArgb("#1ABC1D08");
        }
        else
        {
            StatusText = "Deu Entrada";
            StatusColor = Color.FromArgb("#4D9C24");
            StatusFrameBackground = Color.FromArgb("#1A17BC08");
        }
    }

    /// <summary>
    /// Atualiza marcadores, ponteiro e progresso do anel de turno.
    /// </summary>
    private void UpdateRing(DateTime? nowBrtOpt = null)
    {
        if (_user is null)
            return;
        var rawStart = ToBrt(_user.StartTime);
        var rawEnd = ToBrt(_user.EndTime);
        var nowBrt = nowBrtOpt ?? GetNowBrt();
        bool completedToday = _shiftCompleted && _lastOutBrt.HasValue && _lastOutBrt.Value.Date == nowBrt.Date;
        DateTime visualNow = completedToday ? _lastOutBrt!.Value : nowBrt;
        int startSec12 = ((rawStart.Hour % 12) * 3600) + rawStart.Minute * 60 + rawStart.Second;
        _ring.StartMarkerAngleDeg = (float)(startSec12 / 43200.0 * 360.0);
        int endSec12 = ((rawEnd.Hour % 12) * 3600) + rawEnd.Minute * 60 + rawEnd.Second;
        _ring.EndMarkerAngleDeg = (float)(endSec12 / 43200.0 * 360.0);
        var tolerance = GetTolerance();
        float tolMins = (float)tolerance.TotalMinutes;
        if (tolMins <= 0f)
            tolMins = 10f;
        float sweepDeg = tolMins;
        _ring.StartMarkerSweepDeg = sweepDeg;
        _ring.EndMarkerSweepDeg = sweepDeg;
        int nowSec12 = ((visualNow.Hour % 12) * 3600) + visualNow.Minute * 60 + visualNow.Second;
        _ring.HandAngleDeg = (float)(nowSec12 / 43200.0 * 360.0);
        if (completedToday)
        {
            _ring.StartMarkerColor = _ring.CompletedColor;
            _ring.EndMarkerColor = _ring.CompletedColor;
        }
        else if (_isInNow)
        {
            _ring.StartMarkerColor = _ring.HandColor;
            _ring.EndMarkerColor = _ring.BaseEndMarkerColor;
        }
        else
        {
            _ring.StartMarkerColor = ApplyTwelveHoursFade(_ring.BaseStartMarkerColor, rawStart, visualNow);
            _ring.EndMarkerColor = ApplyTwelveHoursFade(_ring.BaseEndMarkerColor, rawEnd, visualNow);
        }
        _ring.ShiftCompleted = completedToday;
        UpdateRingProgress(visualNow);
        ShiftRing.Invalidate();
    }

    /// <summary>
    /// Calcula a próxima ocorrência de um horário-modelo a partir de um instante.
    /// </summary>
    /// <returns>Próxima ocorrência do horário-modelo.</returns>
    private static DateTime NextOccurrence(DateTime markerTemplate, DateTime now)
    {
        var candidate = new DateTime(now.Year, now.Month, now.Day, markerTemplate.Hour, markerTemplate.Minute, markerTemplate.Second, DateTimeKind.Unspecified);
        return candidate <= now ? candidate.AddDays(1) : candidate;
    }

    /// <summary>
    /// Aplica efeito de fade se o marcador estiver a mais de 12h do ponteiro.
    /// </summary>
    /// <returns>Cor original ou com alpha reduzido.</returns>
    private static Color ApplyTwelveHoursFade(Color baseColor, DateTime marker, DateTime now)
    {
        var next = NextOccurrence(marker, now);
        var diff = next - now;
        if (diff > TimeSpan.FromHours(12))
            return baseColor.WithAlpha(0.5f);
        return baseColor;
    }

    /// <summary>
    /// Atualiza a barra de progresso e a barra extra (overtime) do anel do turno.
    /// </summary>
    private void UpdateRingProgress(DateTime visualNow)
    {
        if (_user is null)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        if (!_isInNow && !_shiftCompleted)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        if (!_entryBrt.HasValue)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        var entryBrt = _entryBrt.Value;
        var rawStart = ToBrt(_user.StartTime);
        var rawEnd = ToBrt(_user.EndTime);
        bool overnight = rawEnd.TimeOfDay <= rawStart.TimeOfDay;
        var entryDay = ToBrt(entryBrt).Date;
        var visualDay = ToBrt(visualNow).Date;
        if ((!overnight && visualDay > entryDay) || (overnight && visualDay > entryDay.AddDays(1)))
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        float gap = _ring.MarkerGapDeg;
        float startDial = NormalizeAngle(_ring.StartMarkerAngleDeg);
        float endDial = NormalizeAngle(_ring.EndMarkerAngleDeg);
        float handDial = NormalizeAngle(_ring.HandAngleDeg);
        float spanSE = (endDial - startDial + 360f) % 360f;
        const float eps = 0.01f;
        if (spanSE <= 2f * gap + eps)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        float relFromStart(float angle) => (NormalizeAngle(angle) - startDial + 360f) % 360f;
        int entrySec12 = ((entryBrt.Hour % 12) * 3600) + entryBrt.Minute * 60 + entryBrt.Second;
        float entryDial = NormalizeAngle((float)(entrySec12 / 43200.0 * 360.0));
        float relEntry = relFromStart(entryDial);
        float relHand = relFromStart(handDial);
        bool entryInsideArc = relEntry <= spanSE;
        float uStartInside = gap;
        float uEndInside = spanSE - gap;
        float entryUraw = entryInsideArc ? relEntry : uStartInside;
        float entryU = Clamp(entryUraw, uStartInside, uEndInside);
        var shiftStartForEntry = GetShiftStartForHit(rawStart, rawEnd, entryBrt);
        var shiftEndForEntry = GetShiftEndForHit(rawStart, rawEnd, entryBrt);
        var effectiveSpan = shiftEndForEntry - entryBrt;
        if (effectiveSpan <= TimeSpan.Zero)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        var elapsed = visualNow - entryBrt;
        if (elapsed <= TimeSpan.Zero)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }
        double factor = elapsed.TotalSeconds / effectiveSpan.TotalSeconds;
        if (factor < 0)
            factor = 0;
        if (factor > 1)
            factor = 1;
        float insideEndU = entryU + (float)(factor * (uEndInside - entryU));
        if (insideEndU > entryU + eps)
        {
            _ring.ProgressStartAngleDeg = NormalizeAngle(startDial + entryU);
            _ring.ProgressEndAngleDeg = NormalizeAngle(startDial + insideEndU);
            _ring.ProgressColor = _shiftCompleted ? _ring.CompletedColor : Color.FromArgb("#1e2d69");
            _ring.ShowProgress = true;
        }
        else
        {
            _ring.ShowProgress = false;
        }
        var tol = GetTolerance();
        var endLimit = shiftEndForEntry + tol;
        bool afterRealEnd = visualNow > endLimit;
        float distFromEnd = (relHand - spanSE + 360f) % 360f;
        bool passedGap = distFromEnd > gap + eps;
        if (afterRealEnd && passedGap)
        {
            float uOvertimeMin = spanSE + gap;
            float overtimeStartU = MathF.Max(uOvertimeMin, insideEndU);
            float overtimeEndU = relHand;
            float overtimeSpan = (overtimeEndU - overtimeStartU + 360f) % 360f;
            if (overtimeSpan > eps)
            {
                _ring.OvertimeStartAngleDeg = NormalizeAngle(startDial + overtimeStartU);
                _ring.OvertimeEndAngleDeg = NormalizeAngle(startDial + overtimeEndU);
                var baseColor = _shiftCompleted ? _ring.CompletedColor : Color.FromArgb("#1e2d69");
                _ring.OvertimeColor = baseColor.WithAlpha(0.2f);
                _ring.ShowOvertime = true;
            }
            else
            {
                _ring.ShowOvertime = false;
            }
        }
        else
        {
            _ring.ShowOvertime = false;
        }
    }

    /// <summary>
    /// Limita um valor float ao intervalo informado.
    /// </summary>
    /// <returns>Valor clamped entre min e max.</returns>
    private static float Clamp(float value, float min, float max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }

    /// <summary>
    /// Calcula a distância angular mínima entre dois ângulos.
    /// </summary>
    /// <returns>Distância angular em graus.</returns>
    private static float AngularDistance(float a, float b)
    {
        float na = NormalizeAngle(a);
        float nb = NormalizeAngle(b);
        float diff = MathF.Abs(na - nb) % 360f;
        return MathF.Min(diff, 360f - diff);
    }

    /// <summary>
    /// Normaliza um ângulo para o intervalo [0, 360).
    /// </summary>
    /// <returns>Ângulo normalizado.</returns>
    private static float NormalizeAngle(float angle)
    {
        var a = angle % 360f;
        if (a < 0)
            a += 360f;
        return a;
    }

    /// <summary>
    /// Calcula o span bruto entre início e fim de turno.
    /// </summary>
    /// <returns>Duração do turno.</returns>
    private static TimeSpan GetShiftSpan(DateTime start, DateTime end)
    {
        return end - start;
    }

    /// <summary>
    /// Verifica se um horário está dentro do intervalo do turno com margem extra.
    /// </summary>
    /// <returns>True se estiver no intervalo estendido.</returns>
    private bool IsWithinShiftInterval(DateTime t, DateTime start, DateTime end)
    {
        if (end <= start)
            end = end.AddDays(1);
        var margin = TimeSpan.FromHours(12);
        var windowEnd = end + margin;
        return t >= start && t <= windowEnd;
    }

    /// <summary>
    /// Verifica se um horário está dentro da janela de tolerância ao redor de um marcador.
    /// </summary>
    /// <returns>True se estiver dentro da tolerância.</returns>
    private bool IsWithinPunchWindow(DateTime now, DateTime marker)
    {
        var tolerance = GetTolerance();
        var diff = (now - marker).Duration();
        return diff <= tolerance;
    }

    /// <summary>
    /// Inicia o timer que atualiza o relógio e o estado da tela.
    /// </summary>
    private void StartRingTimer()
    {
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick -= OnTimerTick;
        _timer.Tick += OnTimerTick;
        if (!_timer.IsRunning)
            _timer.Start();
    }

    /// <summary>
    /// Tick do timer que avança o relógio e atualiza UI e regras de saída.
    /// </summary>
    private void OnTimerTick(object? sender, EventArgs e)
    {
        var nowBrtReal = GetNowBrt();
        bool completedToday = _shiftCompleted && _lastOutBrt.HasValue && _lastOutBrt.Value.Date == nowBrtReal.Date;
        var visualNow = completedToday ? _lastOutBrt!.Value : nowBrtReal;
        NowBrtLabel.Text = visualNow.ToString("HH:mm:ss", PtBr);
        UpdateRing(nowBrtReal);
        UpdateActionButtonState(nowBrtReal);
        if (completedToday)
            StopRingTimer();
    }

    /// <summary>
    /// Interrompe o timer usado pelo relógio.
    /// </summary>
    private void StopRingTimer()
    {
        if (_timer is null)
            return;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }

    /// <summary>
    /// Notifica alterações nos bindings de cabeçalho.
    /// </summary>
    private void NotifyHeaderBindings()
    {
        OnPropertyChanged(nameof(GreetingPrefix));
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

    /// <summary>
    /// Notifica alterações nos bindings do botão principal.
    /// </summary>
    private void NotifyActionButtonBindings()
    {
        OnPropertyChanged(nameof(IsActionButtonEnabled));
        OnPropertyChanged(nameof(ActionButtonText));
        OnPropertyChanged(nameof(ActionButtonBackground));
    }

    /// <summary>
    /// Mapeia uma RecentActivity para o view model exibido na lista.
    /// </summary>
    /// <returns>View model preenchido para a lista de atividades.</returns>
    private RecentItemVM MapActivityToVM(RecentActivity a)
    {
        var t = ToBrt(a.PunchTime);
        var isIn = a.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase);
        var vm = new RecentItemVM
        {
            PunchTimeBrt = t.ToString("HH:mm 'BRT'", PtBr),
            ShiftTypeText = a.ShiftType,
            DateShort = t.ToString("dd/MM/yy", PtBr),
            PunchTypeText = isIn ? "Entrada" : "Saída",
            PunchIconKey = isIn ? "Entrada" : "Saída"
        };
        var fs = new FormattedString();
        fs.Spans.Add(new Span
        {
            FontAttributes = FontAttributes.Bold,
            Text = isIn ? "Entrada" : "Saída",
            TextColor = Colors.Black
        });
        if (_user is not null)
        {
            var rawStart = ToBrt(_user.StartTime);
            var rawEnd = ToBrt(_user.EndTime);
            var tol = GetTolerance();
            if (isIn)
            {
                var shiftStart = GetShiftStartForHit(rawStart, rawEnd, t);
                var startLimit = shiftStart + tol;
                if (t > startLimit)
                {
                    var delayMinutes = (int)Math.Round((t - startLimit).TotalMinutes);
                    if (delayMinutes < 1)
                        delayMinutes = 1;
                    fs.Spans.Add(new Span
                    {
                        FontAttributes = FontAttributes.Bold,
                        Text = $" (Atraso) +{delayMinutes} min",
                        TextColor = Color.FromArgb("#962020")
                    });
                    vm.PunchTypeText = $"Entrada (Atraso) +{delayMinutes} min";
                }
            }
            else
            {
                GetShiftWindowForNow(t, out var shiftStartForHit, out var shiftEndForHit);
                var endLimit = shiftEndForHit + tol;
                if (t > endLimit)
                {
                    var delayMinutes = (int)Math.Round((t - endLimit).TotalMinutes);
                    if (delayMinutes < 1)
                        delayMinutes = 1;
                    fs.Spans.Add(new Span
                    {
                        FontAttributes = FontAttributes.Bold,
                        Text = $" (Tardia) +{delayMinutes} min",
                        TextColor = Color.FromArgb("#962020")
                    });
                    vm.PunchTypeText = $"Saída (Tardia) +{delayMinutes} min";
                }
            }
        }
        vm.PunchTypeFormatted = fs;
        return vm;
    }

    /// <summary>
    /// Obtém a TimeZoneInfo correspondente ao fuso BRT.
    /// </summary>
    /// <returns>TimeZoneInfo de BRT.</returns>
    private static TimeZoneInfo GetBrt()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
        catch
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
    }

    /// <summary>
    /// Converte um DateTime para BRT respeitando o Kind.
    /// </summary>
    /// <returns>Instante convertido para BRT.</returns>
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

    /// <summary>
    /// Interpreta um DateTime como BRT e converte para UTC.
    /// </summary>
    /// <returns>Instante convertido para UTC.</returns>
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

    /// <summary>
    /// Obtém o TimeSpan de tolerância configurado no usuário.
    /// </summary>
    /// <returns>Tolerância em minutos convertida para TimeSpan.</returns>
    private TimeSpan GetTolerance()
    {
        int minutes = _user?.ToleranceMinutes ?? 10;
        if (minutes <= 0)
            minutes = 10;
        return TimeSpan.FromMinutes(minutes);
    }

    /// <summary>
    /// Gera saídas automáticas às 23:59 para dias anteriores com entrada sem saída.
    /// </summary>
    /// <returns>True se alguma saída automática foi criada.</returns>
    private bool EnsureAutoOutForPastOpenDays(DateTime nowBrt)
    {
        if (_user is null || _user.RecentActivities is null || _user.RecentActivities.Count == 0)
            return false;
        var startBrt = ToBrt(_user.StartTime);
        var endBrt = ToBrt(_user.EndTime);
        if (endBrt.TimeOfDay <= startBrt.TimeOfDay)
            return false;
        var today = nowBrt.Date;
        var acts = _user.RecentActivities
            .Select(a => new { Act = a, TimeBrt = ToBrt(a.PunchTime) })
            .ToList();
        var groups = acts
            .GroupBy(x => x.TimeBrt.Date)
            .Where(g => g.Key < today)
            .OrderBy(g => g.Key)
            .ToList();
        bool createdAny = false;
        foreach (var g in groups)
        {
            bool hasIn = g.Any(x => x.Act.PunchType.Equals("IN", StringComparison.OrdinalIgnoreCase));
            bool hasOut = g.Any(x => x.Act.PunchType.Equals("OUT", StringComparison.OrdinalIgnoreCase));
            if (!hasIn || hasOut)
                continue;
            var shiftDay = g.Key;
            var outBrt = new DateTime(shiftDay.Year, shiftDay.Month, shiftDay.Day, 23, 59, 0, DateTimeKind.Unspecified);
            var outUtc = ToUtcAssumingBrt(outBrt);
            var activity = new RecentActivity
            {
                PunchTime = outUtc,
                PunchType = "OUT",
                ShiftType = _user.ShiftType
            };
            _user.RecentActivities.Add(activity);
            createdAny = true;
        }
        return createdAny;
    }

    /// <summary>
    /// Calcula a janela de início e fim do turno para o dia de nowBrt.
    /// </summary>
    private void GetShiftWindowForNow(DateTime nowBrt, out DateTime shiftStartToday, out DateTime shiftEndToday)
    {
        var templateStart = ToBrt(_user.StartTime);
        var templateEnd = ToBrt(_user.EndTime);
        var baseDate = nowBrt.Date;
        var start = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, templateStart.Hour, templateStart.Minute, templateStart.Second, DateTimeKind.Unspecified);
        var endSameDay = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, templateEnd.Hour, templateEnd.Minute, templateEnd.Second, DateTimeKind.Unspecified);
        if (templateEnd.TimeOfDay <= templateStart.TimeOfDay)
        {
            shiftStartToday = start;
            shiftEndToday = endSameDay.AddDays(1);
        }
        else
        {
            shiftStartToday = start;
            shiftEndToday = endSameDay;
        }
    }

    /// <summary>
    /// Calcula o início do turno ao qual uma batida pertence, tratando turnos overnight.
    /// </summary>
    /// <returns>Início do turno referente à batida.</returns>
    private DateTime GetShiftStartForHit(DateTime rawStart, DateTime rawEnd, DateTime t)
    {
        bool overnight = rawEnd.TimeOfDay <= rawStart.TimeOfDay;
        var candidate = new DateTime(t.Year, t.Month, t.Day, rawStart.Hour, rawStart.Minute, rawStart.Second, DateTimeKind.Unspecified);
        if (!overnight)
            return candidate;
        if (t.TimeOfDay < rawStart.TimeOfDay)
            candidate = candidate.AddDays(-1);
        return candidate;
    }

    /// <summary>
    /// Calcula o fim do turno ao qual uma batida pertence, tratando turnos overnight.
    /// </summary>
    /// <returns>Fim do turno referente à batida.</returns>
    private DateTime GetShiftEndForHit(DateTime rawStart, DateTime rawEnd, DateTime hitBrt)
    {
        var start = GetShiftStartForHit(rawStart, rawEnd, hitBrt);
        var end = new DateTime(start.Year, start.Month, start.Day, rawEnd.Hour, rawEnd.Minute, rawEnd.Second, DateTimeKind.Unspecified);
        if (end <= start)
            end = end.AddDays(1);
        return end;
    }

    public sealed class RecentItemVM
    {
        public string PunchTypeText { get; set; } = "";
        public string PunchTimeBrt { get; set; } = "";
        public string ShiftTypeText { get; set; } = "";
        public string DateShort { get; set; } = "";
        public FormattedString PunchTypeFormatted { get; set; } = new();
        public string PunchIconKey { get; set; } = "";
    }
}
