using Camera.MAUI;
using CommunityToolkit.Maui.Core;
using FaceONNX;
using System.Diagnostics;
using System.Drawing;


namespace DeltaFour.Maui.Pages;

public partial class FaceRegisterPage : ContentPage
{
    static Dictionary<string, bool> NeedUserAuthorization = new()
{
    { "MicrophoneInfo", false },
    { "StorageInfo", false },
    { "CameraInfo", false },
};


    public FaceRegisterPage()
    {
        InitializeComponent();
        Overlay.Drawable = new FaceOverlay(() => lastFaces);

    }

    private async void PermissionCheck()
    {
        var requestPerm = await CameraView.RequestPermissions(
                       withMic: NeedUserAuthorization["MicrophoneInfo"],
                       withStorageWrite: NeedUserAuthorization["StorageInfo"]
                   );

        NeedUserAuthorization["MicrophoneInfo"] = await Permissions.CheckStatusAsync<Permissions.Microphone>() == PermissionStatus.Granted;
        NeedUserAuthorization["StorageInfo"] = await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;
        NeedUserAuthorization["CameraInfo"] = await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted;

        if (NeedUserAuthorization.ContainsValue(false))
        {
            await DisplayAlert("Permissão negada", "Sem acesso à câmera.", "OK");
            await Navigation.PopAsync();
            return;
        }

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(1);
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (CameraView.Cameras.Count > 0)
            {
                try
                {
                    CameraView.Camera = CameraView.Cameras[1];
                    await CameraView.StartCameraAsync(new Size(1920, 1080));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
                
        });
    }
    sealed class FaceOverlay : IDrawable
    {
        readonly Func<List<RectangleF>> getFaces;
        public FaceOverlay(Func<List<RectangleF>> provider) => getFaces = provider;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var faces = getFaces();
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 3;

            // assume overlay mesmo tamanho do preview
            foreach (var f in faces)
                canvas.DrawRectangle(f);
        }
    }
}
