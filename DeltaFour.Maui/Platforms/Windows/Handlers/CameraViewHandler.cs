#if WINDOWS
using DeltaFour.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using WinFontWeights = Microsoft.UI.Text.FontWeights;
using WinGrid = Microsoft.UI.Xaml.Controls.Grid;
using WinHorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using WinTextAlignment = Microsoft.UI.Xaml.TextAlignment;
using WinVerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;

namespace DeltaFour.Maui.Handlers
{
    public partial class CameraViewHandler : ViewHandler<CameraView, WinGrid>
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

        protected override WinGrid CreatePlatformView()
        {
            var root = new WinGrid
            {
                Background = new WinBrush(ColorHelper.FromArgb(255, 13, 13, 15))
            };

            var panel = new StackPanel
            {
                Spacing = 8,
                HorizontalAlignment = WinHorizontalAlignment.Center,
                VerticalAlignment = WinVerticalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Preview da câmera",
                Foreground = new WinBrush(ColorHelper.FromArgb(255, 233, 213, 255)),
                FontSize = 18,
                FontWeight = WinFontWeights.SemiBold,
                HorizontalAlignment = WinHorizontalAlignment.Center,
                TextAlignment = WinTextAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Modo visual para Windows",
                Foreground = new WinBrush(ColorHelper.FromArgb(255, 189, 169, 216)),
                FontSize = 13,
                HorizontalAlignment = WinHorizontalAlignment.Center,
                TextAlignment = WinTextAlignment.Center
            });

            root.Children.Add(panel);
            return root;
        }

        static void MapIsActive(CameraViewHandler handler, CameraView view)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Opacity = view.IsActive ? 1 : 0.65;
        }

        static void MapLens(CameraViewHandler handler, CameraView view)
        {
        }

        static void MapStart(CameraViewHandler handler, CameraView view, object? args)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Opacity = 1;
        }

        static void MapStop(CameraViewHandler handler, CameraView view, object? args)
        {
            if (handler.PlatformView is not null)
                handler.PlatformView.Opacity = 0.65;
        }
    }
}
#endif
