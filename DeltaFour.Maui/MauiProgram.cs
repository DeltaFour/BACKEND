using CommunityToolkit.Maui;
using DeltaFour.Maui;
using DeltaFour.Maui.Services;
using DeltaFour.Maui.Controls;
#if ANDROID
using DeltaFour.Maui.Handlers;
#endif
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).Logging.AddDebug();
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler(typeof(CameraView), typeof(CameraViewHandler));
#endif
        });
        builder.Services.AddSingleton<ISession, Session>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
