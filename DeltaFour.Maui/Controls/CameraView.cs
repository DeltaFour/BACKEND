using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
