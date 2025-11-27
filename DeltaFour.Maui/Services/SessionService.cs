using DeltaFour.Maui.Local;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Services
{
    /// <summary>
    /// Representa o estado de sessão da aplicação.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Usuário autenticado atualmente na sessão.
        /// </summary>
        LocalUser? CurrentUser { get; set; }

        /// <summary>
        /// Indica se há um usuário autenticado.
        /// </summary>
        bool IsAuthenticated { get; set; }

        /// <summary>
        /// Indica se a sessão está em modo de demonstração de tempo.
        /// </summary>
        bool IsDemoTime { get; set; }

        /// <summary>
        /// Instante atual simulado em BRT quando em modo demo.
        /// </summary>
        DateTime? DemoNowBrt { get; set; }

        /// <summary>
        /// Token JWT atual da sessão.
        /// </summary>
        string? JwtToken { get; set; }

        /// <summary>
        /// Refresh token atual da sessão.
        /// </summary>
        string? RefreshToken { get; set; }

        /// <summary>
        /// Carrega o estado da sessão a partir do armazenamento seguro.
        /// </summary>
        /// <returns>Tarefa assíncrona de carregamento.</returns>
        Task LoadAsync();

        /// <summary>
        /// Persiste o estado atual da sessão no armazenamento seguro.
        /// </summary>
        /// <returns>Tarefa assíncrona de gravação.</returns>
        Task SaveAsync();

        /// <summary>
        /// Limpa completamente a sessão e remove dados persistidos.
        /// </summary>
        /// <returns>Tarefa assíncrona de limpeza.</returns>
        Task ClearAsync();
    }

    /// <summary>
    /// Implementação de sessão persistida usando SecureStorage.
    /// </summary>
    public sealed class Session : ISession
    {
        const string JwtKey = "session.jwt";
        const string RefreshKey = "session.refresh";
        const string UserKey = "session.user";
        const string AuthKey = "session.authenticated";
        const string DemoKey = "session.demo";
        const string DemoNowKey = "session.demo.nowbrt";

        /// <summary>
        /// Indica se há um usuário autenticado.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Usuário autenticado atualmente na sessão.
        /// </summary>
        public LocalUser? CurrentUser { get; set; }

        /// <summary>
        /// Token JWT atual da sessão.
        /// </summary>
        public string? JwtToken { get; set; }

        /// <summary>
        /// Refresh token atual da sessão.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Indica se a sessão está em modo de demonstração de tempo.
        /// </summary>
        public bool IsDemoTime { get; set; }

        /// <summary>
        /// Instante atual simulado em BRT quando em modo demo.
        /// </summary>
        public DateTime? DemoNowBrt { get; set; }

        /// <summary>
        /// Carrega tokens, usuário e flags de sessão do SecureStorage.
        /// </summary>
        /// <returns>Tarefa assíncrona de carregamento.</returns>
        public async Task LoadAsync()
        {
            JwtToken = await SecureStorage.GetAsync(JwtKey);
            RefreshToken = await SecureStorage.GetAsync(RefreshKey);
            var authStr = await SecureStorage.GetAsync(AuthKey);
            IsAuthenticated = authStr == "1";
            var userJson = await SecureStorage.GetAsync(UserKey);
            if (!string.IsNullOrWhiteSpace(userJson))
            {
                try
                {
                    CurrentUser = JsonSerializer.Deserialize<LocalUser>(userJson);
                }
                catch
                {
                    CurrentUser = null;
                }
            }
            var demoStr = await SecureStorage.GetAsync(DemoKey);
            IsDemoTime = demoStr == "1";
            var demoNowStr = await SecureStorage.GetAsync(DemoNowKey);
            if (DateTime.TryParse(demoNowStr, out var demoNow))
                DemoNowBrt = demoNow;
        }

        /// <summary>
        /// Persiste tokens, usuário e flags de sessão no SecureStorage.
        /// </summary>
        /// <returns>Tarefa assíncrona de gravação.</returns>
        public async Task SaveAsync()
        {
            if (!string.IsNullOrEmpty(JwtToken))
                await SecureStorage.SetAsync(JwtKey, JwtToken);
            else
                SecureStorage.Remove(JwtKey);
            if (!string.IsNullOrEmpty(RefreshToken))
                await SecureStorage.SetAsync(RefreshKey, RefreshToken);
            else
                SecureStorage.Remove(RefreshKey);
            await SecureStorage.SetAsync(AuthKey, IsAuthenticated ? "1" : "0");
            await SecureStorage.SetAsync(DemoKey, IsDemoTime ? "1" : "0");
            if (DemoNowBrt.HasValue)
                await SecureStorage.SetAsync(DemoNowKey, DemoNowBrt.Value.ToString("O"));
            else
                SecureStorage.Remove(DemoNowKey);
            if (CurrentUser != null)
            {
                var json = JsonSerializer.Serialize(CurrentUser);
                await SecureStorage.SetAsync(UserKey, json);
            }
            else
            {
                SecureStorage.Remove(UserKey);
            }
        }

        /// <summary>
        /// Limpa tokens, usuário e flags de sessão do estado atual e do SecureStorage.
        /// </summary>
        /// <returns>Tarefa assíncrona de limpeza.</returns>
        public async Task ClearAsync()
        {
            JwtToken = null;
            RefreshToken = null;
            CurrentUser = null;
            IsAuthenticated = false;
            IsDemoTime = false;
            DemoNowBrt = null;
            SecureStorage.Remove(JwtKey);
            SecureStorage.Remove(RefreshKey);
            SecureStorage.Remove(UserKey);
            SecureStorage.Remove(AuthKey);
            SecureStorage.Remove(DemoKey);
            SecureStorage.Remove(DemoNowKey);
            await Task.CompletedTask;
        }
    }
}
