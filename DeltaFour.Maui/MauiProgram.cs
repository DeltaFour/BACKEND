using CommunityToolkit.Maui;
using DeltaFour.Maui;
using DeltaFour.Maui.Services;
using DeltaFour.Maui.Controls;
#if ANDROID
using DeltaFour.Maui.Handlers;
#endif
using Microsoft.Extensions.Logging;

/// <summary>
/// Configura e constrói o MauiApp da aplicação.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Registra serviços, handlers, fontes, logging e retorna o MauiApp configurado.
    /// </summary>
    /// <returns>Instância configurada de <see cref="MauiApp"/>.</returns>
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
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = false
            }; HttpMessageHandler handler = httpClientHandler;
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
