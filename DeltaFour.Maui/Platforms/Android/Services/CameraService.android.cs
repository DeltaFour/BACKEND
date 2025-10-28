#if ANDROID
using Android.Content;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using DeltaFour.Maui.Controls;
using Java.Lang;
using Java.Util.Concurrent;
namespace DeltaFour.Maui.Platforms.Android.Services
{
    public partial class CameraService
    {
        Context? context;
        ILifecycleOwner? lifecycleOwner;
        ICamera camera;
        ResolutionSelector? resolutionSelector;
        ProcessCameraProvider? cameraProvider;
        IExecutorService? cameraExecutor;
        PreviewView? previewView;
        Preview? preview;
        ImageCapture? imageCapture;

        public async Task<bool> EnsurePermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }
        private static readonly global::Android.Util.Size ScreenSize = new(640, 480);
        public void StartCameraAsync(
                    PreviewView pv,
                    Context ctx,
                    ILifecycleOwner owner,
                    CameraLens lens)
        {
            context = ctx;
            lifecycleOwner = owner;
            previewView = pv;

            cameraExecutor ??= Executors.NewSingleThreadExecutor();

            var cameraProviderFuture = ProcessCameraProvider.GetInstance(context);
            cameraProviderFuture.AddListener(new Runnable(() =>
            {
                cameraProvider = (ProcessCameraProvider)cameraProviderFuture.Get();

                resolutionSelector?.Dispose();
                resolutionSelector = new ResolutionSelector.Builder()
                    .SetResolutionStrategy(new ResolutionStrategy(ScreenSize, ResolutionStrategy.FallbackRuleClosestHigherThenLower))
                    .Build();

                preview?.Dispose();
                preview = new Preview.Builder()
                    .SetResolutionSelector(resolutionSelector)
                    .SetTargetFrameRate(new global::Android.Util.Range(30, 60))
                    .Build();

                preview.SetSurfaceProvider(cameraExecutor, previewView.SurfaceProvider);

                try
                {
                    cameraProvider.UnbindAll();
                    var selector = lens == CameraLens.Front
                         ? CameraSelector.DefaultFrontCamera
                         : CameraSelector.DefaultBackCamera;

                    camera = cameraProvider.BindToLifecycle(lifecycleOwner, selector, preview);
                    System.Diagnostics.Trace.WriteLine($"Camera ligou {camera.CameraInfo}");

                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Trace.WriteLine($"Camera start failed: {e.Message}");
                }
            }), ContextCompat.GetMainExecutor(context));
        }
        public void StopCamera()
        {
            try { cameraProvider?.UnbindAll(); } catch { }
        }

    }
}
#endif