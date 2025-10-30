// MainPage.xaml.cs
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
