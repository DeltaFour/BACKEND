#if ANDROID
using System.Collections.Generic;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;
#endif

namespace DeltaFour.Maui.Controls
{
    public enum CameraLens { Front, Back }

    public class CameraView : View
    {
        public const string StartCommand = "Start";
        public const string StopCommand = "Stop";

        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(CameraView), false);

        public static readonly BindableProperty LensProperty =
            BindableProperty.Create(nameof(Lens), typeof(CameraLens), typeof(CameraView), CameraLens.Front);

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public CameraLens Lens
        {
            get => (CameraLens)GetValue(LensProperty);
            set => SetValue(LensProperty, value);
        }
#if ANDROID
        public event Action<IList<MLFace>, int, int, int>? FacesDetected;

        internal void OnFacesDetected(IList<MLFace> faces, int width, int height, int rotation)
            => FacesDetected?.Invoke(faces, width, height, rotation);
#endif
    }
}
