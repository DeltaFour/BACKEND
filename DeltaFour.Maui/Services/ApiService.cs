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

    }

    public sealed class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<LocalUser?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username obrigatório.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password obrigatório.", nameof(password));

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

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsync("api/v1/auth/logout", content: null, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        public async Task<bool> CanPunchAsync(DateTime timeBrt, bool punchingOut, CancellationToken cancellationToken = default)
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

        private sealed class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}

