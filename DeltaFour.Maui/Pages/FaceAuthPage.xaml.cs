namespace DeltaFour.Maui.Pages;

public partial class FaceRegisterPage : ContentPage
{
    public FaceRegisterPage()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Preview.IsActive = true;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Preview.IsActive = false;
    }

}
