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
using Xamarin.Google.MLKit.Vision.Face;
using Exception = Java.Lang.Exception;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;

namespace DeltaFour.Maui.Platforms.Android.Services
{
    public partial class CameraService
    {
        Context? context;
        ILifecycleOwner? lifecycleOwner;
        ICamera? camera;
        ResolutionSelector? resolutionSelector;
        ProcessCameraProvider? cameraProvider;
        IExecutorService? cameraExecutor;
        PreviewView? previewView;
        Preview? preview;
        ImageAnalysis? imageAnalysis;
        FaceAnalyzer? faceAnalyzer;
        IFaceDetector? faceDetector;

        public Action<IList<MLFace>, int, int, int>? FacesDetected { get; set; }

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

                // ---------- ResolutionSelector ----------
                resolutionSelector?.Dispose();
                resolutionSelector = new ResolutionSelector.Builder()
                    .SetResolutionStrategy(
                        new ResolutionStrategy(
                            ScreenSize,
                            ResolutionStrategy.FallbackRuleClosestHigherThenLower))
                    .Build();

                // ---------- Preview ----------
                preview?.Dispose();
                preview = new Preview.Builder()
                    .SetResolutionSelector(resolutionSelector)
                    .SetTargetFrameRate(new global::Android.Util.Range(30, 60))
                    .Build();

                preview.SetSurfaceProvider(cameraExecutor, previewView.SurfaceProvider);

                // ---------- ImageAnalysis + FaceAnalyzer ----------
                imageAnalysis?.ClearAnalyzer();
                imageAnalysis?.Dispose();

                imageAnalysis = new ImageAnalysis.Builder()
                    .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                    .SetTargetResolution(ScreenSize)
                    .Build();

                // Detector do ML Kit (criado uma vez)
                faceDetector ??= FaceDetection.GetClient(
                    new FaceDetectorOptions.Builder()
                        .SetPerformanceMode(FaceDetectorOptions.PerformanceModeFast)
                        .EnableTracking() // opcional
                        .Build());

                faceAnalyzer ??= new FaceAnalyzer(
                    faceDetector,
                    (faces, w, h, rotation) =>
                    {
                        // repassa para quem estiver inscrito no serviço
                        FacesDetected?.Invoke(faces, w, h, rotation);
                    });

                imageAnalysis.SetAnalyzer(cameraExecutor, faceAnalyzer);

                try
                {
                    cameraProvider.UnbindAll();

                    var selector = lens == CameraLens.Front
                        ? CameraSelector.DefaultFrontCamera
                        : CameraSelector.DefaultBackCamera;

                    // bind preview + análise
                    camera = cameraProvider.BindToLifecycle(
                        lifecycleOwner,
                        selector,
                        preview,
                        imageAnalysis);

                    System.Diagnostics.Trace.WriteLine($"Camera ligou {camera.CameraInfo}");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine($"Camera start failed: {e.Message}");
                }
            }), ContextCompat.GetMainExecutor(context));
        }

        public void StopCamera()
        {
            try
            {
                imageAnalysis?.ClearAnalyzer();
                cameraProvider?.UnbindAll();
            }
            catch { }
        }
    }
}
#endif
