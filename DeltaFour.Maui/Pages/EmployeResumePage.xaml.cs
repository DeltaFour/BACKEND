using DeltaFour.Maui.Local;
using DeltaFour.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
namespace DeltaFour.Maui.Pages;
public partial class EmployeResume : ContentPage
{
    /// <summary>
    /// Sessão atual da aplicação.
    /// </summary>
    private ISession? session;
    /// <summary>
    /// Usuário local carregado na tela.
    /// </summary>
    private IApiService apiService;
    private LocalUser? _user;
    private bool _firstAppearing = true;
    /// <summary>
    /// Indica se o usuário está em expediente no turno atual.
    /// </summary>
    private bool _isInNow;
    /// <summary>
    /// Indica se o expediente atual já foi concluído.
    /// </summary>
    private bool _shiftCompleted;
    /// <summary>
    /// Momento da última saída do expediente atual em BRT.
    /// </summary>
    private DateTime? _lastOutBrt;
    /// <summary>
    /// Indica se existe pelo menos uma batida registrada.
    /// </summary>
    private bool _hasLastPunch;
    /// <summary>
    /// Indica se a última batida geral foi de entrada.
    /// </summary>
    private bool _lastWasIn;
    /// <summary>
    /// Indica se a última batida geral foi de saída.
    /// </summary>
    private bool _lastWasOut;
    /// <summary>
    /// Horário da batida de entrada do expediente atual em BRT.
    /// </summary>
    private DateTime? _entryBrt;
    /// <summary>
    /// Tempo BRT simulado usado para debug de rotação do relógio.
    /// </summary>
    private DateTime _currentNowBrt;
    /// <summary>
    /// Multiplicador de tempo para simulação (1x normal, valores maiores aceleram).
    /// </summary>
    private int _timeMultiplier = 1;
    /// <summary>
    /// Lista de atividades recentes exibidas na UI.
    /// </summary>
    public ObservableCollection<RecentItemVM> RecentItems { get; } = new();
    /// <summary>
    /// Texto de saudação com o nome do usuário.
    /// </summary>
    public string GreetingText { get; private set; } = "";
    /// <summary>
    /// Horário de início do turno em BRT, formatado.
    /// </summary>
    public string StartTimeBrt { get; private set; } = "";
    /// <summary>
    /// Horário de término do turno em BRT, formatado.
    /// </summary>
    public string EndTimeBrt { get; private set; } = "";
    /// <summary>
    /// Texto que descreve o tipo de turno.
    /// </summary>
    public string ShiftTypeText { get; private set; } = "";
    public string GreetingPrefix { get; private set; } = "Olá ";

    /// <summary>
    /// Data em que o turno começou a valer.
    /// </summary>
    public string StartedOnDate { get; private set; } = "";
    /// <summary>
    /// Texto que descreve a janela de trabalho (início e fim).
    /// </summary>
    public string WorkWindowText { get; private set; } = "";
    /// <summary>
    /// Nome da empresa exibido no cabeçalho.
    /// </summary>
    public string CompanyNameText { get; private set; } = "";
    /// <summary>
    /// Texto de status da última batida (entrada ou saída).
    /// </summary>
    public string StatusText { get; private set; } = "";
    /// <summary>
    /// Cor do texto de status no cabeçalho.
    /// </summary>
    public Color StatusColor { get; private set; } = Colors.Transparent;
    /// <summary>
    /// Cor de fundo do frame de status no cabeçalho.
    /// </summary>
    public Color StatusFrameBackground { get; private set; } = Colors.Transparent;
    /// <summary>
    /// Texto exibido no botão principal de ação (entrada/saída).
    /// </summary>
    public string ActionButtonText { get; private set; } = "";
    /// <summary>
    /// Cor de fundo do botão principal de ação.
    /// </summary>
    public Color ActionButtonBackground { get; private set; } = Colors.Transparent;
    /// <summary>
    /// Indica se o botão principal de ação está habilitado.
    /// </summary>
    public bool IsActionButtonEnabled { get; private set; }
    /// <summary>
    /// BindableProperty para o texto da última batida exibido na UI.
    /// </summary>
    public static readonly BindableProperty LastPunchTextProperty =
        BindableProperty.Create(nameof(LastPunchText), typeof(string), typeof(EmployeResume), default(string));
    /// <summary>
    /// Texto formatado com o horário da última batida registrada.
    /// </summary>
    public string LastPunchText
    {
        get => (string)GetValue(LastPunchTextProperty);
        private set => SetValue(LastPunchTextProperty, value);
    }
    /// <summary>
    /// Drawable responsável por desenhar o relógio/anel do turno.
    /// </summary>
    private readonly ShiftRingDrawable _ring = new();
    /// <summary>
    /// Timer usado para atualizar o relógio e o estado da tela.
    /// </summary>
    private IDispatcherTimer? _timer;
    /// <summary>
    /// Cultura pt-BR usada para formatação de datas e horas.
    /// </summary>
    public static readonly CultureInfo PtBr = new("pt-BR");
    /// <summary>
    /// Construtor da página de resumo do empregado.
    /// </summary>
    public EmployeResume()
    {
        InitializeComponent();
        BindingContext = this;
        HandlerChanged += OnHandlerChanged;
        ShiftRing.Drawable = _ring;
    }
    /// <summary>
    /// Manipula o clique no botão de logout, limpando sessão e navegando para login.
    /// </summary>
    async void OnLogoutClicked(object? sender, EventArgs e)
    {
        if (!TryResolveDependencies())
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
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

        session!.IsAuthenticated = false;
        session.CurrentUser = null;
        _user = null;
        RecentItems.Clear();

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private bool TryResolveDependencies()
    {
        if (session != null && apiService != null)
            return true;

        var services =
            Handler?.MauiContext?.Services ??
            Application.Current?.Handler?.MauiContext?.Services;

        if (services == null)
            return false;

        session ??= services.GetRequiredService<ISession>();
        apiService ??= services.GetRequiredService<IApiService>();

        return true;
    }
    protected override bool OnBackButtonPressed()
    {
#if ANDROID
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool sair = await DisplayAlert(
                "Sair",
                "Deseja sair?",
                "Sim",
                "Não");

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
    private void UpdateGreetingPrefix(DateTime nowBrt)
    {
        var baseGreetings = new[]
        {
        "Olá ",
        "Bem-vindo "
        };

        var candidates = new List<string>(baseGreetings);

        var hour = nowBrt.Hour;

        if (hour >= 5 && hour < 12)
        {
            candidates.Add("Bom dia ");
        }
        else if (hour >= 12 && hour < 18)
        {
            candidates.Add("Boa tarde ");
        }
        else
        {
            candidates.Add("Boa noite ");
        }

        var index = Random.Shared.Next(candidates.Count);
        GreetingPrefix = candidates[index];

        OnPropertyChanged(nameof(GreetingPrefix));
    }
    /// <summary>
    /// Evento de ciclo de vida chamado quando a página aparece.
    /// </summary>
    /// 
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

            var nowBrt = ToBrt(DateTime.UtcNow);
            _currentNowBrt = nowBrt;
            FillAllFromUser(nowBrt);
        }
        catch (ApiUnavailableException)
        {
            await DisplayAlert("Aviso",
                "Servidor indisponível para atualizar suas informações de ponto.",
                "OK");
        }
        catch (ApiException ex)
        {
            Trace.WriteLine($"[EmployeResume] Erro ao atualizar usuário: {ex}");
        }
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        _currentNowBrt = ToBrt(DateTime.UtcNow);

        if (TryResolveDependencies())
            LoadUserIfChanged();

        var nowBrt = GetNowBrt();
        UpdateGreetingPrefix(nowBrt);
        UpdateRing(nowBrt);
        UpdateActionButtonState(nowBrt);
        UpdateLblHoje();

        if (_isInNow && !_shiftCompleted)
            StartRingTimer();
        else
            StopRingTimer();
        if (_firstAppearing)
        {
            _firstAppearing = false;
            return;
        }

        _ = RefreshUserFromApiAsync();
    }

    /// <summary>
    /// Evento de ciclo de vida chamado quando a página desaparece.
    /// </summary>
    protected override void OnDisappearing()
    {
        StopRingTimer();
        base.OnDisappearing();
    }
    /// <summary>
    /// Handler chamado quando o Handler da página muda, usado para tentar recarregar a sessão.
    /// </summary>
    private void OnHandlerChanged(object? s, EventArgs e)
    {
        if (TryResolveDependencies())
            LoadUserIfChanged();
    }
    /// <summary>
    /// Carrega o usuário da sessão se a referência mudou e normaliza as datas.
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

        var nowBrt = _currentNowBrt != default
            ? _currentNowBrt
            : ToBrt(DateTime.UtcNow);

        FillAllFromUser(nowBrt);
    }
    /// <summary>
    /// Preenche todo o estado da tela com base no usuário e no horário atual em BRT.
    /// </summary>
    private void FillAllFromUser(DateTime nowBrt)
    {
        if (_user is null) return;

        var startTemplate = ToBrt(_user.StartTime);
        var endTemplate = ToBrt(_user.EndTime);

        var rawStart = startTemplate;
        var rawEnd = endTemplate;

        GetShiftWindowForNow(nowBrt, out var shiftStartToday, out var shiftEndToday);
        var tol = GetTolerance();

        // início da janela (entrada antecipada, com tolerância)
        var windowStart = shiftStartToday - tol;
        // fim da janela ALINHADO com a lógica de tardia do botão:
        // shiftEnd + tol + 12h
        var lateLimit = shiftEndToday + tol + TimeSpan.FromHours(12);

        GreetingText = _user.Name;
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
        LastPunchText = lastOverall.TimeBrt.ToString("'Às' HH:mm:ss", PtBr);

        // AGORA: inclui também saídas tardias até shiftEnd + tol + 12h
        var todaysActs = actsWithBrt
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
                // IN + OUT (normal ou tardia) dentro do mesmo ciclo -> turno concluído
                _isInNow = false;
                _shiftCompleted = true;
                _entryBrt = lastInToday.TimeBrt;
                _lastOutBrt = lastToday.TimeBrt;
            }
            else if (lastTodayIsIn)
            {
                // Tem entrada, mas ainda não tem saída para este ciclo
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

        var ringNow = _shiftCompleted && _lastOutBrt.HasValue
            ? _lastOutBrt.Value
            : nowBrt;

        UpdateRing(ringNow);
        UpdateActionButtonState(nowBrt);
        NotifyHeaderBindings();

        if (_isInNow && !_shiftCompleted)
            StartRingTimer();
        else
            StopRingTimer();
    }

    /// <summary>
    /// Atualiza o label de "Hoje" com a data atual em pt-BR.
    /// </summary>
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
                new Span { Text = nowBrt.ToString("yyyy", PtBr), FontAttributes = FontAttributes.Bold, TextColor = primary }
            }
        };
    }
    /// <summary>
    /// Atualiza estado, texto e cor do botão principal de ação conforme horário atual.
    /// </summary>
    /// <summary>
    /// Atualiza estado, texto e cor do botão principal (Entrada/Saída)
    /// com base na hora atual em BRT e na situação do turno.
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

        // Se estou em expediente, uso a DATA do turno ao qual a entrada pertence.
        // Se não estou em expediente, uso o turno "de hoje" (padrão recorrente).
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
            // Turno já fechado (OUT registrado para esse turno)
            IsActionButtonEnabled = false;
            ActionButtonText = "Expediente Encerrado";
            ActionButtonBackground = Color .FromArgb("#a1a1a1");
        }
        else if (!_isInNow)
        {
            // Ainda NÃO deu entrada neste turno
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
                // Janela de entrada passou, mas ainda estamos no "miolo" do turno
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Entrada (Atraso)";
                ActionButtonBackground = Color.FromArgb("#4D9C24");
            }
            else
            {
                // Turno deste ciclo já passou inteiro -> aguarda próximo ciclo
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Expediente";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
        }
        else
        {
            // Já está em expediente neste turno (_isInNow == true)
            if (beforeExitWin)
            {
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Saída";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
            else if (inExitWindow)
            {
                // Dentro da janela normal de saída
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Saída";
                ActionButtonBackground = Color.FromArgb("#962020");
            }
            else if (afterExitWin && nowBrt <= shiftEnd + tol + TimeSpan.FromHours(12))
            {
                // Saída tardia até, no máximo, 12h após fim+tol desse MESMO turno
                IsActionButtonEnabled = true;
                ActionButtonText = "Dar Saída (Tardia)";
                ActionButtonBackground = Color.FromArgb("#962020");
            }
            else
            {
                // Já passou inclusive a janela de tardia -> considera turno fechado
                IsActionButtonEnabled = false;
                ActionButtonText = "Aguardar Expediente";
                ActionButtonBackground = Color.FromArgb("#a1a1a1");
            }
        }

        NotifyActionButtonBindings();
    }

    /// <summary>
    /// Manipula o clique no botão principal de ação e registra nova batida.
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
        } else
        {
            await DisplayAlert("Aviso", "API ALLOWED CORRIJIDO.", "OK");
        }
        var typeParam = Uri.EscapeDataString(newType);
        var timeParam = Uri.EscapeDataString(
            nowBrt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture));

        var route = $"//MainTabs/FaceRegisterPage?punchType={typeParam}&timeBrt={timeParam}";
        await Shell.Current.GoToAsync(route);
        return;
        var utcNow = ToUtcAssumingBrt(nowBrt);
        var activity = new RecentActivity
        {
            PunchTime = utcNow,
            PunchType = newType,
            ShiftType = _user.ShiftType
        };
        _user.RecentActivities ??= new();
        _user.RecentActivities.Add(activity);
        FillAllFromUser(nowBrt);
    }
    /// <summary>
    /// Manipula o evento de pressionar o botão principal, acelerando o tempo.
    /// </summary>
    private void OnActionButtonPressed(object? sender, EventArgs e)
    {
        _timeMultiplier = 1040;
    }
    /// <summary>
    /// Manipula o evento de soltar o botão principal, voltando o tempo ao normal.
    /// </summary>
    private void OnActionButtonReleased(object? sender, EventArgs e)
    {
        _timeMultiplier = 1;
    }
    /// <summary>
    /// Atualiza o texto e cores de status do cabeçalho com base na última batida.
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
    /// Atualiza o desenho do anel/relógio (marcadores, ponteiro e cores).
    /// </summary>
    private void UpdateRing(DateTime? nowBrtOpt = null)
    {
        if (_user is null)
            return;

        var rawStart = ToBrt(_user.StartTime);
        var rawEnd = ToBrt(_user.EndTime);


        DateTime visualNow;
        if (_shiftCompleted && _lastOutBrt.HasValue)
            visualNow = _lastOutBrt.Value;
        else
            visualNow = nowBrtOpt ?? GetNowBrt();

        int startSec12 = ((rawStart.Hour % 12) * 3600) + rawStart.Minute * 60 + rawStart.Second;
        _ring.StartMarkerAngleDeg = (float)(startSec12 / 43200.0 * 360.0);

        int endSec12 = ((rawEnd.Hour % 12) * 3600) + rawEnd.Minute * 60 + rawEnd.Second;
        _ring.EndMarkerAngleDeg = (float)(endSec12 / 43200.0 * 360.0);


        var tolerance = GetTolerance();
        float tolMins = (float)tolerance.TotalMinutes;
        if (tolMins <= 0f) tolMins = 10f;
        float sweepDeg = tolMins;
        _ring.StartMarkerSweepDeg = sweepDeg;
        _ring.EndMarkerSweepDeg = sweepDeg;


        int nowSec12 = ((visualNow.Hour % 12) * 3600) + visualNow.Minute * 60 + visualNow.Second;
        _ring.HandAngleDeg = (float)(nowSec12 / 43200.0 * 360.0);

        if (_shiftCompleted)
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
            _ring.StartMarkerColor = ApplyTwelveHoursFade(
                _ring.BaseStartMarkerColor,
                rawStart,
                visualNow);

            _ring.EndMarkerColor = ApplyTwelveHoursFade(
                _ring.BaseEndMarkerColor,
                rawEnd,
                visualNow);
        }

        _ring.ShiftCompleted = _shiftCompleted;

        UpdateRingProgress(visualNow);

        ShiftRing.Invalidate();
    }

    /// <summary>
    /// Calcula a próxima ocorrência de um horário-modelo a partir de um instante.
    /// </summary>
    private static DateTime NextOccurrence(DateTime markerTemplate, DateTime now)
    {
        var candidate = new DateTime(
            now.Year, now.Month, now.Day,
            markerTemplate.Hour, markerTemplate.Minute, markerTemplate.Second,
            DateTimeKind.Unspecified);
        return candidate <= now
            ? candidate.AddDays(1)
            : candidate;
    }
    /// <summary>
    /// Aplica efeito de fade (50% alpha) se o marcador estiver a mais de 12h do ponteiro.
    /// </summary>
    private static Color ApplyTwelveHoursFade(Color baseColor, DateTime marker, DateTime now)
    {
        var next = NextOccurrence(marker, now);
        var diff = next - now;
        if (diff > TimeSpan.FromHours(12))
            return baseColor.WithAlpha(0.5f);
        return baseColor;
    }
    /// <summary>
    /// Atualiza a barra de progresso e a barra extra (tardia) do anel com base no horário atual.
    /// </summary>
    private void UpdateRingProgress(DateTime visualNow)
    {
        if (_user is null)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }

        // Só faz sentido mostrar barra se:
        // - está em expediente, ou
        // - o expediente atual já foi concluído (Expediente Encerrado).
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

        float gap = _ring.MarkerGapDeg;

        float startDial = NormalizeAngle(_ring.StartMarkerAngleDeg);
        float endDial = NormalizeAngle(_ring.EndMarkerAngleDeg);
        float handDial = NormalizeAngle(_ring.HandAngleDeg);

        float spanSE = (endDial - startDial + 360f) % 360f;
        const float eps = 0.01f;

        // Se o arco útil não existe, não desenha nada
        if (spanSE <= 2f * gap + eps)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }

        float relFromStart(float angle) => (NormalizeAngle(angle) - startDial + 360f) % 360f;

        int entrySec12 = ((entryBrt.Hour % 12) * 3600) + entryBrt.Minute * 60 + entryBrt.Second;
        float entryDial = NormalizeAngle((float)(entrySec12 / 43200.0 * 360.0));

        float relEntry = relFromStart(entryDial);   // 0..360
        float relHand = relFromStart(handDial);    // 0..360

        bool entryInsideArc = relEntry <= spanSE;
        bool handInsideArc = relHand <= spanSE;

        float uStartInside = gap;            // início útil = start + gap
        float uEndInside = spanSE - gap;   // fim útil   = end   - gap
        float uOvertimeMin = spanSE + gap;   // overtime   = end   + gap

        // Entrada fora do arco E ponteiro fora do arco -> nada a mostrar
        if (!entryInsideArc && !handInsideArc)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }

        // Posição da entrada (se fora do arco, projeta pro início útil)
        float entryUraw = entryInsideArc ? relEntry : uStartInside;
        float handUraw = relHand;

        float entryU = Clamp(entryUraw, uStartInside, uEndInside);

        float deltaFromEntry = (handUraw - entryU + 360f) % 360f;
        if (deltaFromEntry <= eps)
        {
            _ring.ShowProgress = false;
            _ring.ShowOvertime = false;
            return;
        }

        // -------- Barra normal (do início até o fim do turno) --------

        float insideEndU = MathF.Min(handUraw, uEndInside);

        if (insideEndU > entryU + eps)
        {
            _ring.ProgressStartAngleDeg = NormalizeAngle(startDial + entryU);
            _ring.ProgressEndAngleDeg = NormalizeAngle(startDial + insideEndU);

            _ring.ProgressColor = _shiftCompleted
                ? _ring.CompletedColor          // cinza se já deu saída
                : Color.FromArgb("#1e2d69");    // azul durante o expediente

            _ring.ShowProgress = true;
        }
        else
        {
            _ring.ShowProgress = false;
        }

        // -------- Barra extra (tardia) --------

        // Usa o dia da ENTRADA pra achar o fim real daquele turno
        var rawStart = ToBrt(_user.StartTime);
        var rawEnd = ToBrt(_user.EndTime);
        var tol = GetTolerance();

        var shiftEndForEntry = GetShiftEndForHit(rawStart, rawEnd, entryBrt);
        var endLimit = shiftEndForEntry + tol;

        // Overtime só se o horário visual (agora ou saída) passou do fim real + tolerância
        bool afterRealEnd = visualNow > endLimit;

        if (afterRealEnd && handUraw > uOvertimeMin + eps)
        {
            float overtimeStartU = MathF.Max(uOvertimeMin, entryU);
            float overtimeEndU = handUraw;
            float overtimeSpan = overtimeEndU - overtimeStartU;

            if (overtimeSpan > eps)
            {
                _ring.OvertimeStartAngleDeg = NormalizeAngle(startDial + overtimeStartU);
                _ring.OvertimeEndAngleDeg = NormalizeAngle(startDial + overtimeEndU);

                var baseColor = _shiftCompleted
                    ? _ring.CompletedColor
                    : Color.FromArgb("#1e2d69");

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
    /// Limita um valor float a um intervalo mínimo e máximo.
    /// </summary>
    private static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    /// <summary>
    /// Calcula a distância angular mínima entre dois ângulos em graus.
    /// </summary>
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
    private static float NormalizeAngle(float angle)
    {
        var a = angle % 360f;
        if (a < 0) a += 360f;
        return a;
    }
    /// <summary>
    /// Calcula o span bruto entre início e fim de turno.
    /// </summary>
    private static TimeSpan GetShiftSpan(DateTime start, DateTime end)
    {
        return end - start;
    }
    /// <summary>
    /// Verifica se um DateTime está dentro do intervalo de turno considerando margem longa.
    /// </summary>
    private bool IsWithinShiftInterval(DateTime t, DateTime start, DateTime end)
    {
        if (end <= start)
            end = end.AddDays(1);
        var margin = TimeSpan.FromHours(12);
        var windowEnd = end + margin;
        return t >= start && t <= windowEnd;
    }
    /// <summary>
    /// Verifica se um instante está dentro da janela de tolerância em torno de um marcador.
    /// </summary>
    private bool IsWithinPunchWindow(DateTime now, DateTime marker)
    {
        var tolerance = GetTolerance();
        var diff = (now - marker).Duration();
        return diff <= tolerance;
    }
    /// <summary>
    /// Inicia o timer que atualiza o relógio e o estado se o turno não está concluído.
    /// </summary>
    private void StartRingTimer()
    {
        if (_shiftCompleted) return;
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick -= OnTimerTick;
        _timer.Tick += OnTimerTick;
        if (!_timer.IsRunning)
            _timer.Start();
    }
    /// <summary>
    /// Tick do timer que avança o tempo simulado e atualiza UI e regras de saída automática.
    /// </summary>
    private void OnTimerTick(object? sender, EventArgs e)
    {
        var nowBrtReal = GetNowBrt();

        DateTime visualNow;

        if (_shiftCompleted && _lastOutBrt.HasValue)
        {
            visualNow = _lastOutBrt.Value;

            StopRingTimer();
        }
        else
        {
            visualNow = nowBrtReal;
        }

        NowBrtLabel.Text = visualNow.ToString("HH:mm:ss", PtBr);

        UpdateRing(visualNow);

        UpdateActionButtonState(nowBrtReal);
    }

    /// <summary>
    /// Interrompe o timer do relógio.
    /// </summary>
    private void StopRingTimer()
    {
        if (_timer is null) return;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }
    /// <summary>
    /// Dispara notificações de property changed para os bindings de cabeçalho.
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
    /// Dispara notificações de property changed para os bindings do botão principal.
    /// </summary>
    private void NotifyActionButtonBindings()
    {
        OnPropertyChanged(nameof(IsActionButtonEnabled));
        OnPropertyChanged(nameof(ActionButtonText));
        OnPropertyChanged(nameof(ActionButtonBackground));
    }
    /// <summary>
    /// Mapeia uma RecentActivity de domínio para o view model exibido na lista.
    /// </summary>
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
                // ENTRADA (ATRASO)
                var shiftStart = GetShiftStartForHit(rawStart, rawEnd, t);
                var startLimit = shiftStart + tol;

                if (t > startLimit)
                {
                    var delayMinutes = (int)Math.Round((t - startLimit).TotalMinutes);
                    if (delayMinutes < 1) delayMinutes = 1;

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
                // SAÍDA (TARDIA) — usar mesma lógica do botão
                GetShiftWindowForNow(t, out var shiftStartForHit, out var shiftEndForHit);
                var endLimit = shiftEndForHit + tol;

                if (t > endLimit)
                {
                    var delayMinutes = (int)Math.Round((t - endLimit).TotalMinutes);
                    if (delayMinutes < 1) delayMinutes = 1;

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
    private static TimeZoneInfo GetBrt()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); }
    }
    /// <summary>
    /// Converte um DateTime para BRT respeitando o Kind.
    /// </summary>
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
    /// Obtém o TimeSpan de tolerância configurado no usuário, com padrão de 10 minutos.
    /// </summary>
    private TimeSpan GetTolerance()
    {
        int minutes = _user?.ToleranceMinutes ?? 10;
        if (minutes <= 0) minutes = 10;
        return TimeSpan.FromMinutes(minutes);
    }
    /// <summary>
    /// Retorna o horário atual em BRT, avançando tempo simulado conforme multiplicador.
    /// </summary>
    private DateTime GetNowBrt()
    {
        if (_currentNowBrt == default)
        {
            _currentNowBrt = ToBrt(DateTime.UtcNow);
        }
        else
        {
            _currentNowBrt = _currentNowBrt.AddSeconds(_timeMultiplier);
        }
        return _currentNowBrt;
    }
    /// <summary>
    /// Garante que todos os dias ANTES de hoje que tenham pelo menos uma ENTRADA
    /// e nenhuma SAÍDA recebam uma SAÍDA automática às 23:59.
    /// Retorna true se pelo menos uma saída automática foi criada.
    /// Ignora turnos overnight (start > end).
    /// </summary>
    private bool EnsureAutoOutForPastOpenDays(DateTime nowBrt)
    {
        if (_user is null || _user.RecentActivities is null || _user.RecentActivities.Count == 0)
            return false;

        var startBrt = ToBrt(_user.StartTime);
        var endBrt = ToBrt(_user.EndTime);

        // Só auto-fecha para turno simples (não overnight).
        if (endBrt.TimeOfDay <= startBrt.TimeOfDay)
            return false;

        var today = nowBrt.Date;

        var acts = _user.RecentActivities
            .Select(a => new { Act = a, TimeBrt = ToBrt(a.PunchTime) })
            .ToList();

        // Agrupa por dia e pega SOMENTE dias < hoje com IN e sem OUT.
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

            var outBrt = new DateTime(
                shiftDay.Year, shiftDay.Month, shiftDay.Day,
                23, 59, 0,
                DateTimeKind.Unspecified);

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
    /// Calcula janela de início e fim do turno referente ao dia do nowBrt.
    /// </summary>
    private void GetShiftWindowForNow(DateTime nowBrt, out DateTime shiftStartToday, out DateTime shiftEndToday)
    {
        var templateStart = ToBrt(_user.StartTime);
        var templateEnd = ToBrt(_user.EndTime);
        var baseDate = nowBrt.Date;
        var start = new DateTime(
            baseDate.Year, baseDate.Month, baseDate.Day,
            templateStart.Hour, templateStart.Minute, templateStart.Second,
            DateTimeKind.Unspecified);
        var endSameDay = new DateTime(
            baseDate.Year, baseDate.Month, baseDate.Day,
            templateEnd.Hour, templateEnd.Minute, templateEnd.Second,
            DateTimeKind.Unspecified);
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
    private DateTime GetShiftStartForHit(DateTime rawStart, DateTime rawEnd, DateTime t)
    {
        bool overnight = rawEnd.TimeOfDay <= rawStart.TimeOfDay;
        var candidate = new DateTime(
            t.Year, t.Month, t.Day,
            rawStart.Hour, rawStart.Minute, rawStart.Second,
            DateTimeKind.Unspecified);
        if (!overnight)
            return candidate;
        if (t.TimeOfDay < rawStart.TimeOfDay)
            candidate = candidate.AddDays(-1);
        return candidate;
    }
    /// <summary>
    /// Calcula o fim do turno ao qual uma batida pertence, tratando turnos overnight.
    /// </summary>
    private DateTime GetShiftEndForHit(DateTime rawStart, DateTime rawEnd, DateTime hitBrt)
    {
        var start = GetShiftStartForHit(rawStart, rawEnd, hitBrt);
        var end = new DateTime(
            start.Year, start.Month, start.Day,
            rawEnd.Hour, rawEnd.Minute, rawEnd.Second,
            DateTimeKind.Unspecified);
        if (end <= start)
            end = end.AddDays(1);
        return end;
    }
    /// <summary>
    /// View model de um item de atividade recente exibido na lista.
    /// </summary>
    public sealed class RecentItemVM
    {
        /// <summary>
        /// Texto da ação (Entrada, Saída, Entrada Atraso, etc.).
        /// </summary>
        public string PunchTypeText { get; set; } = "";
        /// <summary>
        /// Horário da batida em BRT formatado.
        /// </summary>
        public string PunchTimeBrt { get; set; } = "";
        /// <summary>
        /// Tipo de turno associado à batida.
        /// </summary>
        public string ShiftTypeText { get; set; } = "";
        /// <summary>
        /// Data curta da batida.
        /// </summary>
        public string DateShort { get; set; } = "";
        /// <summary>
        /// Texto formatado com spans coloridos e negrito para tipo/atrasos.
        /// </summary>
        public FormattedString PunchTypeFormatted { get; set; } = new();
        /// <summary>
        /// Chave usada para selecionar o ícone/SVG correto (Entrada ou Saída).
        /// </summary>
        public string PunchIconKey { get; set; } = "";
    }
}
