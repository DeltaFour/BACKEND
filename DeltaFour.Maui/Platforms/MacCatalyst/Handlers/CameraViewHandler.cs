#if MACCATALYST
using DeltaFour.Maui.Controls;
using Microsoft.Maui.Handlers;
using UIKit;

namespace DeltaFour.Maui.Handlers
{
    public partial class CameraViewHandler : ViewHandler<CameraView, UIView>
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

        public CameraViewHandler() : base(Mapper, CommandMapper)
        {
        }

        protected override UIView CreatePlatformView()
        {
            var root = new UIView
            {
                BackgroundColor = UIColor.FromRGB(13, 13, 15)
            };

            var title = new UILabel
            {
                Text = "Preview da câmera",
                TextColor = UIColor.FromRGB(233, 213, 255),
                Font = UIFont.BoldSystemFontOfSize(18),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var subtitle = new UILabel
            {
                Text = "Modo visual para macOS",
                TextColor = UIColor.FromRGB(189, 169, 216),
                Font = UIFont.SystemFontOfSize(13),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var stack = new UIStackView(new UIView[] { title, subtitle })
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.EqualSpacing,
                Spacing = 8,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            root.AddSubview(stack);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                stack.CenterXAnchor.ConstraintEqualTo(root.CenterXAnchor),
                stack.CenterYAnchor.ConstraintEqualTo(root.CenterYAnchor),
                stack.LeadingAnchor.ConstraintGreaterThanOrEqualTo(root.LeadingAnchor, 16),
                stack.TrailingAnchor.ConstraintLessThanOrEqualTo(root.TrailingAnchor, -16)
            });

            return root;
        }

        static void MapIsActive(CameraViewHandler handler, CameraView view)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Alpha = view.IsActive ? 1 : 0.65f;
        }

        static void MapLens(CameraViewHandler handler, CameraView view)
        {
        }

        static void MapStart(CameraViewHandler handler, CameraView view, object? args)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Alpha = 1;
        }

        static void MapStop(CameraViewHandler handler, CameraView view, object? args)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Alpha = 0.65f;
        }
    }
}
#endif
