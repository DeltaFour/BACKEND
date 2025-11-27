using DeltaFour.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DeltaFour.Maui
{
    /// <summary>
    /// Classe principal da aplicação MAUI, responsável por inicializar e decidir a tela inicial.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Provider de serviços usado para resolver dependências.
        /// </summary>
        readonly IServiceProvider _sp;

        /// <summary>
        /// Constrói a aplicação, exibe uma tela de carregamento e inicia a verificação de sessão.
        /// </summary>
        /// <param name="sp">Provider de serviços da aplicação.</param>
        public App(IServiceProvider sp)
        {
            InitializeComponent();
            _sp = sp;
            MainPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#F5F8FB"),
                Content = new Grid
                {
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = "Carregando...",
                            VerticalOptions = LayoutOptions.End,
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0,0,0,40)
                        }
                    }
                }
            };
            _ = InitializeAsync();
        }

        /// <summary>
        /// Decide se deve abrir o Shell ou a tela de Login com base na sessão persistida.
        /// </summary>
        /// <returns>Tarefa assíncrona que representa o fluxo de inicialização.</returns>
        private async Task InitializeAsync()
        {
            var session = _sp.GetRequiredService<ISession>();
            await session.LoadAsync();
            var api = _sp.GetRequiredService<IApiService>();
            Trace.WriteLine($"{session.RefreshToken} teste {session.JwtToken}");
            var hasTokens = !string.IsNullOrWhiteSpace(session.JwtToken) && !string.IsNullOrWhiteSpace(session.RefreshToken);
            if (hasTokens)
            {
                try
                {
                    var ok = await api.CheckSessionAsync();

                    if (!ok)
                    {
                        var refreshed = await api.TryRefreshTokenAsync();
                        if (refreshed)
                            ok = await api.CheckSessionAsync();
                    }

                    if (ok)
                    {
                        try
                        {
                            if (session.CurrentUser is not null)
                            {
                                var updated = await api.RefreshUserAsync(session.CurrentUser);
                                if (updated is not null)
                                {
                                    session.CurrentUser = updated;
                                    await session.SaveAsync();
                                }
                            }
                        }
                        catch (ApiUnavailableException ex)
                        {
                            Trace.WriteLine($"[App] Falha ao atualizar usuário após restaurar sessão (servidor indisponível): {ex}");
                        }
                        catch (ApiException ex)
                        {
                            Trace.WriteLine($"[App] Falha ao atualizar usuário após restaurar sessão: {ex}");
                        }

                        EnterShell();
                        await Shell.Current.GoToAsync("//MainTabs/EmployeResumePage");
                        return;
                    }
                }
                catch (ApiUnavailableException ex)
                {
                    Trace.WriteLine($"[App] Servidor indisponível ao restaurar sessão: {ex}");
                }
                catch (ApiException ex)
                {
                    Trace.WriteLine($"[App] Erro ao restaurar sessão: {ex}");
                }
                await session.ClearAsync();
            }
            ShowLogin();
        }

        /// <summary>
        /// Exibe a página de login como página principal.
        /// </summary>
        private void ShowLogin()
        {
            MainPage = new NavigationPage(_sp.GetRequiredService<LoginPage>());
        }

        /// <summary>
        /// Entra no Shell da aplicação após login bem-sucedido.
        /// </summary>
        public void EnterShell()
        {
            MainPage = _sp.GetRequiredService<AppShell>();
        }

        /// <summary>
        /// Executa logout global limpando a sessão e retornando para a tela de login.
        /// </summary>
        /// <returns>Tarefa assíncrona que representa o fluxo de saída para o login.</returns>
        public async Task ExitToLoginAsync()
        {
            var session = _sp.GetRequiredService<ISession>();
            await session.ClearAsync();
            ShowLogin();
        }
    }
}
