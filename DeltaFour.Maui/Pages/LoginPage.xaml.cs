using DeltaFour.Maui.Local;
using DeltaFour.Maui.Pages;
using DeltaFour.Maui.Services;
using System.Diagnostics;
#if ANDROID
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.ApplicationModel;
using Platform = Microsoft.Maui.ApplicationModel.Platform;
#endif

namespace DeltaFour.Maui
{
    /// <summary>
    /// Página de login responsável por autenticar o usuário e iniciar a sessão.
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        bool _hidePassword = true;
        private ISession? session;
        private readonly IApiService authService;

        /// <summary>
        /// Construtor da página de login, recebendo sessão e serviço de autenticação.
        /// </summary>
        public LoginPage(ISession session, IApiService authService)
        {
            InitializeComponent();
            HandlerChanged += OnHandlerChanged;
            this.authService = authService;
            this.session = session;
        }

#if ANDROID
        /// <summary>
        /// Fecha o teclado virtual no Android.
        /// </summary>
        void DismissKeyboard()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var activity = Platform.CurrentActivity;
                if (activity is null) return;
                var imm = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                var view = activity.CurrentFocus ?? activity.Window?.DecorView;
                if (view is null)
                    view = new Android.Views.View(activity);
                var token = view.WindowToken;
                if (token != null)
                    imm?.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
                view.ClearFocus();
            });
        }
#endif

        /// <summary>
        /// Manipula mudança de handler para tentar resolver a sessão via DI.
        /// </summary>
        private void OnHandlerChanged(object? sender, EventArgs e) => TryResolveSession();

        /// <summary>
        /// Resolve a instância de sessão a partir do serviço de injeção de dependência.
        /// </summary>
        private void TryResolveSession()
        {
            if (session != null) return;
            var sp = Handler?.MauiContext?.Services
                     ?? Application.Current?.Handler?.MauiContext?.Services;
            if (sp != null)
                session = sp.GetRequiredService<ISession>();
        }

        /// <summary>
        /// Alterna a visibilidade da senha no campo de entrada.
        /// </summary>
        void OnTogglePassword(object? sender, EventArgs e)
        {
            _hidePassword = !_hidePassword;
            PasswordEntry.IsPassword = _hidePassword;
            TogglePasswordButton.Text = _hidePassword ? "Mostrar" : "Ocultar";
        }

        /// <summary>
        /// Executa o fluxo de login, autentica na API e entra no Shell se bem sucedido.
        /// </summary>
        async void OnLoginClicked(object? sender, EventArgs e)
        {
#if ANDROID
            DismissKeyboard();
#endif
            var user = UsernameEntry.Text?.Trim();
            var pass = PasswordEntry.Text;
            TryResolveSession();
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                await DisplayAlert("Erro", "Preencha os campos!", "ok");
                return;
            }
            try
            {
                LoginButton.IsEnabled = false;
                BusyIndicator.IsVisible = BusyIndicator.IsRunning = true;
                await Task.Delay(500);
                var apiUser = await authService.LoginAsync(user, pass);
                if (apiUser is null)
                {
                    await DisplayAlert("Erro", "Usuário ou senha inválidos.", "ok");
                    return;
                }
                var nowBrt = ToBrt(DateTime.UtcNow);
                var startBrt = new DateTime(
                    nowBrt.Year, nowBrt.Month, nowBrt.Day,
                    16, 16, 0,
                    DateTimeKind.Unspecified);
                var endBrt = new DateTime(
                    nowBrt.Year, nowBrt.Month, nowBrt.Day,
                    23, nowBrt.Minute, nowBrt.Second,
                    DateTimeKind.Unspecified);
                var outBrt = endBrt.AddDays(-1).AddMinutes(9);
                var mockUser = new LocalUser
                {
                    Name = "Carlos Mendes",
                    ShiftType = "Noturno",
                    StartTime = startBrt,
                    EndTime = endBrt,
                    CompanyName = "DeltaFour Tech",
                    ToleranceMinutes = 10,
                    RecentActivities = new List<RecentActivity>
                    {
                        new()
                        {
                            PunchTime = outBrt,
                            PunchType = "OUT",
                            ShiftType = "Noturno"
                        }
                    }
                };
                session!.CurrentUser = apiUser;
                session.IsAuthenticated = true;
#if DEBUG
                session.IsDemoTime = true;
                session.DemoNowBrt = null;
#else
                session.IsDemoTime = false;
                session.DemoNowBrt = null;
#endif
                await session.SaveAsync();
                ((App)Application.Current).EnterShell();
                await Shell.Current.GoToAsync("//MainTabs/EmployeResumePage");
            }
            catch (ApiUnavailableException)
            {
                await DisplayAlert("Erro", "Servidor indisponível. Verifique sua conexão ou tente novamente mais tarde.", "OK");
            }
            catch (ApiException ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
            }
            finally
            {
                BusyIndicator.IsRunning = false;
                BusyIndicator.IsVisible = false;
                LoginButton.IsEnabled = true;
                UsernameEntry.Text = "";
                PasswordEntry.Text = "";
            }
        }

        /// <summary>
        /// Obtém a timezone de BRT (Brasil) com fallback de ID.
        /// </summary>
        /// <returns>Instância de TimeZoneInfo representando BRT.</returns>
        private static TimeZoneInfo GetBrt()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); }
            catch { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); }
        }

        /// <summary>
        /// Converte um DateTime para o fuso horário BRT.
        /// </summary>
        /// <returns>Data/hora convertida para BRT.</returns>
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
    }
}
