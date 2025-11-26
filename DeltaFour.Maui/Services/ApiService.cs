using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Local;
using DeltaFour.Maui.Mappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Services
{


    
        public interface IApiService
        {
            Task<LocalUser?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
            Task LogoutAsync(CancellationToken cancellationToken = default);
            Task<bool> CanPunchAsync(DateTime timeBrt, bool punchingOut, CancellationToken cancellationToken = default);
            Task<bool> PunchInAsync(
                DateTime timeBrt,
                string imageBase64,
                string shiftType,
                string type,
                double latitude,
                double longitude,
                CancellationToken cancellationToken = default);
        Task<LocalUser?> RefreshUserAsync(LocalUser currentUser, CancellationToken cancellationToken = default);

    }

    public sealed class ApiService : IApiService
        {
            private readonly HttpClient _httpClient;

            public ApiService(HttpClient httpClient)
            {
                _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            }

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

                    using var response = await _httpClient.PostAsync("api/v1/user/punch-in", content, cancellationToken);

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

                var refreshDto = JsonSerializer.Deserialize<ApiUserRefreshDto>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (refreshDto is null)
                    return currentUser;

                UserMapper.ApplyRefresh(currentUser, refreshDto);

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


        private sealed class LoginRequest
            {
                public string Email { get; set; } = "";
                public string Password { get; set; } = "";
            }
        }
    

    public class ApiException : Exception
    {
        public ApiException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }

    public sealed class ApiUnavailableException : ApiException
    {
        public ApiUnavailableException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}

