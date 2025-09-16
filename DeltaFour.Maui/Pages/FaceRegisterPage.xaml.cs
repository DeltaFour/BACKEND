using Camera.MAUI;
using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Helpers;
using FaceONNX;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
    readonly FaceDetector _detector = new();
    CancellationTokenSource? _cts;
    readonly string _snapPath = Path.Combine(FileSystem.CacheDirectory, "preview.png");


    public FaceRegisterPage()
    {
        InitializeComponent();

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
                    await CameraView.StartCameraAsync(new Microsoft.Maui.Graphics.Size(1280, 720));
                    _cts = new CancellationTokenSource();
                    _ = Task.Run(() => DetectionLoopAsync(_cts.Token));

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

        });
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts?.Cancel();
    }
    async Task DetectionLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // pega snapshot em memória
                var snapshot = CameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
                if (snapshot is not StreamImageSource sis)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                await using var snapStream = await sis.Stream(token);
                if (snapStream == null)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                // copie pra memória (pra poder salvar/debug e também ler no ImageSharp)
                using var ms = new MemoryStream();
                await snapStream.CopyToAsync(ms, token);
                ms.Position = 0;

#if DEBUG
                var debugPath = Path.Combine(FileSystem.CacheDirectory, $"debug_{DateTime.Now:HHmmssfff}.png");
                File.WriteAllBytes(debugPath, ms.ToArray());
                ms.Position = 0;
#endif

                using var img = SixLabors.ImageSharp.Image.Load<Rgb24>(ms);
                // roda detector
                var rawFaces = _detector.Forward(img.ToBgrPlanes());
                var best = rawFaces.OrderByDescending(f => f.Score).FirstOrDefault();
                var faces = Array.Empty<FaceBox>();

                if (best != null && best.Score > 0.7f)
                {
                    var sx = (float)(Overlay.Width / img.Width);
                    var sy = (float)(Overlay.Height / img.Height);

                    var r = best.Rectangle; // System.Drawing.Rectangle do FaceONNX
                    faces = new[]
                    {
                    new FaceBox
                    {
                        Bounds = new RectF(
                            (float)(r.X * sx),
                            (float)(r.Y * sy),
                            (float)(r.Width * sx),
                            (float)(r.Height * sy)
                        ),
                        Score = best.Score
                    }
                };
                }

                // desenha overlay
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Overlay.Drawable = new FaceOverlayDrawable(faces);
                    Overlay.Invalidate();
                });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Trace.WriteLine($"detecção: {ex.Message}");
            }

            await Task.Delay(1500, token);
        }
    }
    class FaceOverlayDrawable : IDrawable
    {
        readonly FaceBox[] _faces;
        public FaceOverlayDrawable(FaceBox[] faces) => _faces = faces;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;
            var rect = new RectF(100, 150, 200, 120);
            canvas.DrawRectangle(rect);

            canvas.FontColor = Colors.Yellow;
            canvas.DrawString("DEBUG", rect.X, rect.Y - 20, HorizontalAlignment.Left);
            foreach (var f in _faces)
            {
                var r = f.Bounds;
                canvas.DrawRectangle(r);
                Trace.WriteLine(f.Score);
                canvas.FontColor = Colors.Purple;
                canvas.DrawString(
                    $"{f.Score:F2}",
                    r.X,
                    r.Y - 20,
                    HorizontalAlignment.Left
                );
            }
        }
    }
}
