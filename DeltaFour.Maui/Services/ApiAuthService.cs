using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Local;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Services
{
    public interface IApiAuthService
    {
        Task<LocalUser?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
        Task LogoutAsync(CancellationToken cancellationToken = default);

    }

    public sealed class ApiAuthService : IApiAuthService
    {
        private readonly HttpClient _httpClient;

        public ApiAuthService(HttpClient httpClient)
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

            var request = new LoginRequest
            {
                Email = username,
                Password = password
            };

            var json = JsonSerializer.Serialize(request);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("api/auth/login", content, cancellationToken);

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
            // Se sua API não exige body para o logout:
            using var response = await _httpClient.PostAsync(
                "api/auth/logout",
                content: null,
                cancellationToken);

            // Se quiser tratar manualmente, troque por if (!response.IsSuccessStatusCode) ...
            response.EnsureSuccessStatusCode();
        }

        private sealed class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}

