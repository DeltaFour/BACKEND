#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using DeltaFour.Maui.Controls;
using Java.IO;
using Java.Lang;
using Java.Util.Concurrent;
using Xamarin.Google.MLKit.Vision.Face;
using Exception = Java.Lang.Exception;
using FileIO = System.IO.File;
using JFile = Java.IO.File;
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
        ImageCapture? imageCapture;

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
                imageCapture?.Dispose();
                imageCapture = new ImageCapture.Builder()
                    .SetTargetResolution(ScreenSize)
                    .SetCaptureMode(ImageCapture.CaptureModeMinimizeLatency)
                    .Build();
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
                        imageAnalysis,
                        imageCapture);

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
        public Task<string?> CapturePhotoBase64Async(CancellationToken cancellationToken = default)
        {
            if (imageCapture == null || context == null || cameraExecutor == null)
                return Task.FromResult<string?>(null);

            var tcs = new TaskCompletionSource<string?>();

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    tcs.TrySetCanceled(cancellationToken);
                });
            }

            var fileName = $"punch_{Guid.NewGuid():N}.jpg";

            var file = new JFile(context.CacheDir, fileName);

            var outputOptions = new ImageCapture.OutputFileOptions.Builder(file).Build();

            imageCapture.TakePicture(
                outputOptions,
                cameraExecutor,
                new ImageSavedCallback(
                    file.AbsolutePath,
                    path =>
                    {
                        try
                        {
                            var bytes = FileIO.ReadAllBytes(path);
                            var base64 = Convert.ToBase64String(bytes);
                            tcs.TrySetResult(base64);

                            FileIO.Delete(path);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine($"CapturePhotoBase64Async read error: {ex.Message}");
                            tcs.TrySetResult(null);
                        }
                    },
                    ex =>
                    {
                        System.Diagnostics.Trace.WriteLine($"CapturePhotoBase64Async failed: {ex.Message}");
                        tcs.TrySetResult(null);
                    }));

            return tcs.Task;
        }

        sealed class ImageSavedCallback : Java.Lang.Object, ImageCapture.IOnImageSavedCallback
        {
            readonly string filePath;
            readonly Action<string> onSuccess;
            readonly Action<Exception> onError;

            public ImageSavedCallback(
                string filePath,
                Action<string> onSuccess,
                Action<Exception> onError)
            {
                this.filePath = filePath;
                this.onSuccess = onSuccess;
                this.onError = onError;
            }


            [Register(
                "onImageSaved",
                "(Landroidx/camera/core/ImageCapture$OutputFileResults;)V",
                "GetOnImageSaved_Landroidx_camera_core_ImageCapture_OutputFileResults_Handler")]
            public void OnImageSaved(ImageCapture.OutputFileResults outputFileResults)
            {
                try
                {
                    onSuccess(filePath);
                }
                catch (Exception ex)
                {
                    onError(ex);
                }
            }

            [Register(
                "onError",
                "(Landroidx/camera/core/ImageCaptureException;)V",
                "GetOnError_Landroidx_camera_core_ImageCapture_ImageCaptureException_Handler")]
            public void OnError(ImageCaptureException exception)
            {
                onError(exception);
            }


            [Register(
                "onCaptureStarted",
                "()V",
                "GetOnCaptureStartedHandler")]
            public void OnCaptureStarted()
            {
                // opcional: tocar som de obturador, animar UI, etc.
                System.Diagnostics.Trace.WriteLine("ImageCapture onCaptureStarted");
            }

            [Register(
                "onCaptureProcessProgressed",
                "(I)V",
                "GetOnCaptureProcessProgressed_IHandler")]
            public void OnCaptureProcessProgressed(int progress)
            {
            }

            [Register(
                "onPostviewBitmapAvailable",
                "(Landroid/graphics/Bitmap;)V",
                "GetOnPostviewBitmapAvailable_Landroid_graphics_Bitmap_Handler")]
            public void OnPostviewBitmapAvailable(Bitmap bitmap)
            {
                bitmap?.Dispose();
            }
        }

    }
}
#endif
