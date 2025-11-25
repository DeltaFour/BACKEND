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
#if ANDROID
        builder.Services.AddTransient<CustomAuthHandler>();
#endif
        builder.Services.AddSingleton(sp =>
        {
            var httpClientHandler = new HttpClientHandler();
            HttpMessageHandler handler = httpClientHandler;

#if ANDROID
            handler = new LoggingHandler(handler);
            var authHandler = sp.GetRequiredService<CustomAuthHandler>();
            authHandler.InnerHandler = handler;
            handler = authHandler;
#endif

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://zb3467wz-5212.brs.devtunnels.ms/"),
                Timeout = TimeSpan.FromSeconds(30)
            };

            return client;
        });

        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddTransient<LoginPage>();
        return builder.Build();
    }
}
