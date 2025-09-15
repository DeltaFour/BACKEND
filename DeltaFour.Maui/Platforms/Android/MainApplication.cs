using Android.App;
using Android.Runtime;
// Needed for Taking photo/video
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage, MaxSdkVersion = 32)]

// Add these properties if you would like to filter out devices that do not have cameras, or set to false to make them optional
[assembly: UsesFeature("android.hardware.camera", Required = true)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = true)]
namespace DeltaFour.Maui
{
    [Application]

    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
