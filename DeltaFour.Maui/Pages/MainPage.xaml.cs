// MainPage.xaml.cs

using DeltaFour.Maui.Local;
using DeltaFour.Maui.Pages;

namespace DeltaFour.Maui;

public partial class MainPage : ContentPage
{
    bool _hidePassword = true;

    public MainPage()
    {
        InitializeComponent();
    }

    void OnTogglePassword(object? sender, EventArgs e)
    {
        _hidePassword = !_hidePassword;
        PasswordEntry.IsPassword = _hidePassword;
        TogglePasswordButton.Text = _hidePassword ? "Mostrar" : "Ocultar";
    }

    async void OnLoginClicked(object? sender, EventArgs e)
    {

        var user = UsernameEntry.Text?.Trim();
        var pass = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
           await DisplayAlert("Erro", "Preencha os campos!", "ok");
            return;
        }

        try
        {
            LoginButton.IsEnabled = false;
            BusyIndicator.IsVisible = BusyIndicator.IsRunning = true;

            // TODO: autenticar
            await Task.Delay(500);

            var mockUser = new LocalUser
            {
                Name = "Carlos Mendes",
                ShiftType = "Diurno",
                StartTime = DateTime.Today.AddHours(8).AddMinutes(45),
                EndTime = DateTime.Today.AddHours(17),
                CompanyName = "DeltaFour Tech",
                RecentActivities = new List<RecentActivity>
            {
                new() { PunchTime = DateTime.Today.AddHours(8).AddMinutes(42), PunchType = "IN", ShiftType = "Diurno" },
                new() { PunchTime = DateTime.Today.AddDays(-1).AddHours(17), PunchType = "OUT", ShiftType = "Diurno" },
            }
            };

            await Shell.Current.GoToAsync("EmployeResumePage", new Dictionary<string, object>
            {
                ["user"] = mockUser
            });

        }
        catch (Exception ex)
        {
          await DisplayAlert("Erro", ex.Message, "ok");
        }
        finally
        {
            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
            LoginButton.IsEnabled = true;
        }
    }
}
