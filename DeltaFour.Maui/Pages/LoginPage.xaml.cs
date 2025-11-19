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
    public LoginPage()
    {
        InitializeComponent();
        HandlerChanged += OnHandlerChanged;
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
        {
            session = sp.GetRequiredService<ISession>();
        }
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

            var mockUser = new LocalUser
            {
                Name = "Carlos Mendes",
                ShiftType = "Diurno",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.Today.AddHours(18),
                CompanyName = "DeltaFour Tech",
                RecentActivities = new List<RecentActivity>
            {
                new() { PunchTime = DateTime.Today.AddHours(8).AddMinutes(42), PunchType = "IN", ShiftType = "Diurno" },
                new() { PunchTime = DateTime.Today.AddDays(-1).AddHours(17), PunchType = "OUT", ShiftType = "Diurno" },
                new() { PunchTime = DateTime.Today.AddDays(-1).AddHours(2).AddMinutes(42), PunchType = "IN", ShiftType = "Diurno" },

            }
            };
            session.CurrentUser = mockUser;
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
}
