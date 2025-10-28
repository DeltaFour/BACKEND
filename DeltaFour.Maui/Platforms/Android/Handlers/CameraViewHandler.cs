// Platforms/Android/Handlers/CameraPreviewHandler.Android.cs  (handler + mappers)
#if ANDROID
using AndroidX.Activity;
using AndroidX.Camera.View;
using AndroidX.Lifecycle;
using DeltaFour.Maui.Controls;
using DeltaFour.Maui.Platforms.Android.Services;
using Microsoft.Maui.Handlers;

namespace DeltaFour.Maui.Handlers
{
    public partial class CameraViewHandler : ViewHandler<CameraView, PreviewView>
    {
        public static readonly IPropertyMapper<CameraView, CameraViewHandler> Mapper =
            new PropertyMapper<CameraView, CameraViewHandler>(ViewHandler.ViewMapper)
            {
                [nameof(CameraView.IsActive)] = MapIsActive,
                [nameof(CameraView.Lens)] = MapLens,
            };

        public static readonly CommandMapper<CameraView, CameraViewHandler> CommandMapper =
            new(ViewHandler.ViewCommandMapper)
            {
                [CameraView.StartCommand] = MapStart,
                [CameraView.StopCommand] = MapStop,
            };

        readonly CameraService _service = new();

        public CameraViewHandler() : base(Mapper, CommandMapper) { }

        protected override PreviewView CreatePlatformView()
            => new PreviewView(Context);

        protected override void ConnectHandler(PreviewView platformView)
        {
            base.ConnectHandler(platformView);
            if (VirtualView?.IsActive == true)
                StartCamera();
        }

        protected override void DisconnectHandler(PreviewView platformView)
        {
            _service.StopCamera();
            base.DisconnectHandler(platformView);
        }

        static void MapIsActive(CameraViewHandler h, CameraView v)
        {
            if (h.PlatformView is null) return;
            if (v.IsActive) h.StartCamera();
            else h._service.StopCamera();
        }

        static void MapLens(CameraViewHandler h, CameraView v)
        {
            if (h.PlatformView is null) return;
            if (v.IsActive) h.StartCamera();
        }

        static void MapStart(CameraViewHandler h, CameraView v, object? p)
            => h.StartCamera();

        static void MapStop(CameraViewHandler h, CameraView v, object? p)
            => h._service.StopCamera();

        void StartCamera()
        {
            if (PlatformView is null || VirtualView is null) return;

            var activity = Context as ComponentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity as ComponentActivity;
            var owner = activity as ILifecycleOwner;
            if (owner is null) return;

            _ = _service.EnsurePermissionsAsync().ContinueWith(t =>
            {
                if (!t.Result) return;
                Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                {
                    _service.StartCameraAsync(PlatformView, Context, owner, VirtualView.Lens);
                });
            });
        }
    }
}
#endif
