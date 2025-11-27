using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Local;
using DeltaFour.Maui.Mappers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Services
{
    /// <summary>
    /// Define operações de API para autenticação, ponto e dados do usuário.
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Efetua login do usuário e retorna o usuário local mapeado.
        /// </summary>
        /// <returns>Usuário local autenticado ou null em caso de falha.</returns>
        Task<LocalUser?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Encerra a sessão do usuário na API.
        /// </summary>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se é permitido registrar ponto no horário informado.
        /// </summary>
        /// <returns>True se o registro de ponto é permitido; caso contrário, false.</returns>
        Task<bool> CanPunchAsync(DateTime timeBrt, bool punchingOut, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registra um ponto com foto, localização e tipo de batida.
        /// </summary>
        /// <returns>True se o ponto foi registrado com sucesso; caso contrário, false.</returns>
        Task<bool> PunchInAsync(
            DateTime timeBrt,
            string imageBase64,
            string shiftType,
            string type,
            double latitude,
            double longitude,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza as informações recentes de ponto do usuário.
        /// </summary>
        /// <returns>O usuário atualizado ou o atual em caso de falha.</returns>
        Task<LocalUser?> RefreshUserAsync(LocalUser currentUser, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se a sessão atual é válida no servidor.
        /// </summary>
        /// <returns>True se a sessão é válida; caso contrário, false.</returns>
        Task<bool> CheckSessionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Tenta renovar a sessão usando o refresh token.
        /// </summary>
        /// <returns>True se a sessão foi renovada com sucesso; caso contrário, false.</returns>
        Task<bool> TryRefreshTokenAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementação concreta de IApiService baseada em HttpClient.
    /// </summary>
    public sealed class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ISession _session;

        public ApiService(HttpClient httpClient, ISession session)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Efetua login na API usando email e senha.
        /// </summary>
        /// <returns>Usuário local autenticado ou null se credenciais inválidas.</returns>
        public async Task<LocalUser?> LoginAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username obrigatório.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password obrigatório.", nameof(password));
            try
            {
                var request = new LoginRequest
                {
                    Email = username,
                    Password = password
                };
                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync("api/v1/auth/login", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return null;
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiUser = JsonSerializer.Deserialize<ApiUserDto>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (apiUser is null)
                    return null;
                var localUser = UserMapper.ToLocalUser(apiUser);
                return localUser;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em LoginAsync: {ex}");
                throw new ApiUnavailableException("Não foi possível conectar ao servidor (login). Tente novamente mais tarde.", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em LoginAsync: {ex}");
                throw new ApiException("Erro inesperado ao efetuar login.", ex);
            }
        }

        /// <summary>
        /// Encerra a sessão do usuário na API.
        /// </summary>
        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.PostAsync("api/v1/auth/logout", content: null, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em LogoutAsync: {ex}");
                throw new ApiUnavailableException("Não foi possível conectar ao servidor (logout).", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em LogoutAsync: {ex}");
                throw new ApiException("Erro inesperado ao encerrar sessão.", ex);
            }
        }

        /// <summary>
        /// Verifica com a API se é permitido registrar ponto no horário informado.
        /// </summary>
        /// <returns>True se permitido; caso contrário, false.</returns>
        public async Task<bool> CanPunchAsync(
            DateTime timeBrt,
            bool punchingOut,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var dto = PunchMapper.FromBrtTime(timeBrt, punchingOut);
                var json = JsonSerializer.Serialize(dto);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync("api/v1/user/allowed-punch", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return false;
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    var allowed = JsonSerializer.Deserialize<bool>(responseBody);
                    return allowed;
                }
                catch
                {
                    return false;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em CanPunchAsync: {ex}");
                throw new ApiUnavailableException("Não foi possível validar o registro de ponto (servidor indisponível).", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em CanPunchAsync: {ex}");
                throw new ApiException("Erro inesperado ao validar o registro de ponto.", ex);
            }
        }

        /// <summary>
        /// Envia para a API o registro de ponto com foto, horário e localização.
        /// </summary>
        /// <returns>True se o ponto foi registrado com sucesso; caso contrário, false.</returns>
        public async Task<bool> PunchInAsync(
            DateTime timeBrt,
            string imageBase64,
            string shiftType,
            string type,
            double latitude,
            double longitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var dto = PunchMapper.ToPunchInDto(
                    timeBrt,
                    imageBase64,
                    shiftType,
                    type,
                    latitude,
                    longitude);
                dto.ImageBase64 = $"data:image/jpeg;base64,{imageBase64}";
                var json = JsonSerializer.Serialize(dto);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync("api/v1/user/register-point", content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em PunchInAsync: {ex}");
                throw new ApiUnavailableException("Não foi possível comunicar com o servidor ao registrar o ponto.", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em PunchInAsync: {ex}");
                throw new ApiException("Erro inesperado ao registrar o ponto.", ex);
            }
        }

        /// <summary>
        /// Atualiza as batidas recentes do usuário a partir de endpoint enxuto.
        /// </summary>
        /// <returns>Usuário atualizado ou o mesmo usuário em caso de falha.</returns>
        public async Task<LocalUser?> RefreshUserAsync(
     LocalUser currentUser,
     CancellationToken cancellationToken = default)
        {
            if (currentUser is null)
                throw new ArgumentNullException(nameof(currentUser));

            try
            {
                using var response = await _httpClient.GetAsync(
                    "api/v1/user/refresh-information",
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return currentUser;

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                var refreshDto = JsonSerializer.Deserialize<ApiUserDto>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (refreshDto is null)
                    return currentUser;

                UserMapper.ApplyRefresh(currentUser, refreshDto);

                _session.CurrentUser = currentUser;
                await _session.SaveAsync();

                return currentUser;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em RefreshUserAsync: {ex}");
                throw new ApiUnavailableException(
                    "Não foi possível atualizar as informações do usuário (servidor indisponível).",
                    ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em RefreshUserAsync: {ex}");
                throw new ApiException("Erro inesperado ao atualizar as informações do usuário.", ex);
            }
        }

        /// <summary>
        /// Verifica com o backend se a sessão atual ainda é válida.
        /// </summary>
        /// <returns>True se a sessão é válida; caso contrário, false.</returns>
        public async Task<bool> CheckSessionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    "api/v1/auth/check-session",
                    cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em CheckSessionAsync: {ex}");
                throw new ApiUnavailableException(
                    "Não foi possível verificar a sessão (servidor indisponível).", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em CheckSessionAsync: {ex}");
                throw new ApiException("Erro inesperado ao verificar a sessão.", ex);
            }
        }

        /// <summary>
        /// Solicita ao backend a renovação da sessão usando o refresh token.
        /// </summary>
        /// <returns>True se o token foi renovado com sucesso; caso contrário, false.</returns>
        public async Task<bool> TryRefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/refresh-token");
                request.Headers.Add("X-Bypass-Auth", "true");
                using var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine($"[API] Erro de rede em TryRefreshTokenAsync: {ex}");
                throw new ApiUnavailableException(
                    "Não foi possível renovar a sessão (servidor indisponível).", ex);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[API] Erro inesperado em TryRefreshTokenAsync: {ex}");
                throw new ApiException("Erro inesperado ao renovar a sessão.", ex);
            }
        }

        /// <summary>
        /// DTO de requisição de login enviado ao backend.
        /// </summary>
        private sealed class LoginRequest
        {
            /// <summary>
            /// Email usado para autenticação.
            /// </summary>
            public string Email { get; set; } = "";

            /// <summary>
            /// Password usado para autenticação.
            /// </summary>
            public string Password { get; set; } = "";
        }
    }

    /// <summary>
    /// Exceção base para erros de comunicação ou regra com a API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Cria uma nova instância de ApiException com mensagem e exceção interna opcionais.
        /// </summary>
        public ApiException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exceção para indicar indisponibilidade do servidor ou falha de rede.
    /// </summary>
    public sealed class ApiUnavailableException : ApiException
    {
        /// <summary>
        /// Cria uma nova instância de ApiUnavailableException com mensagem e exceção interna opcionais.
        /// </summary>
        public ApiUnavailableException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
