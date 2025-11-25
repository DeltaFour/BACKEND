// MainPage.xaml.cs

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

namespace DeltaFour.Maui;

public partial class LoginPage : ContentPage
{
    bool _hidePassword = true;
    private ISession? session;
    private readonly IApiService authService;

    public LoginPage(ISession session, IApiService authService)
    {
        InitializeComponent();
        HandlerChanged += OnHandlerChanged;
        this.authService = authService;
        this.session = session;
    }

#if ANDROID
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

    private void OnHandlerChanged(object? sender, EventArgs e) => TryResolveSession();

    private void TryResolveSession()
    {
        if (session != null) return;

        var sp = Handler?.MauiContext?.Services
                 ?? Application.Current?.Handler?.MauiContext?.Services;

        if (sp != null)
            session = sp.GetRequiredService<ISession>();
    }

    void OnTogglePassword(object? sender, EventArgs e)
    {
        _hidePassword = !_hidePassword;
        PasswordEntry.IsPassword = _hidePassword;
        TogglePasswordButton.Text = _hidePassword ? "Mostrar" : "Ocultar";
    }

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
                StartTime = startBrt,   // BRT + Unspecified
                EndTime = endBrt,       // BRT + Unspecified
                CompanyName = "DeltaFour Tech",
                ToleranceMinutes = 10,
                RecentActivities = new List<RecentActivity>
                {
                    new()
                    {
                        PunchTime = outBrt, // BRT + Unspecified
                        PunchType = "OUT",
                        ShiftType = "Noturno"
                    }
                }
            };

            session!.CurrentUser = apiUser;

            session.IsAuthenticated = true;

            ((App)Application.Current).EnterShell();
            await Shell.Current.GoToAsync("//MainTabs/EmployeResumePage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.ToString(), "ok");
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

    // Helpers de fuso BRT (iguais em conceito aos do EmployeResume)
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
}
