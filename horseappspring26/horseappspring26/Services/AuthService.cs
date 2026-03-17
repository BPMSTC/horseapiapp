using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using horseappspring26.Models;
using Microsoft.Extensions.Logging;

namespace horseappspring26.Services;

public class AuthService(IHttpClientFactory httpClientFactory, ITokenStore tokenStore, ILogger<AuthService> logger) : IAuthService
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(tokenStore.AccessToken);

    public bool IsAdmin => HasAdminRole(tokenStore.AccessToken);

    public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("HorseApi");

        var payload = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("login", payload, _jsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorAsync(response, cancellationToken);
            logger.LogWarning("Login failed with status {StatusCode}: {Error}", response.StatusCode, error);
            return false;
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions, cancellationToken);
        if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
        {
            logger.LogWarning("Login succeeded but no access token was returned.");
            return false;
        }

        tokenStore.AccessToken = loginResponse.AccessToken;
        logger.LogInformation("User login successful.");
        return true;
    }

    public void Logout()
    {
        tokenStore.Clear();
        logger.LogInformation("User logged out.");
    }

    private static bool HasAdminRole(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return false;
        }

        try
        {
            var payload = parts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            return ContainsAdminRole(root, "role")
                   || ContainsAdminRole(root, "roles")
                   || ContainsAdminRole(root, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }
        catch
        {
            return false;
        }
    }

    private static bool ContainsAdminRole(JsonElement root, string claimName)
    {
        if (!root.TryGetProperty(claimName, out var claim))
        {
            return false;
        }

        if (claim.ValueKind == JsonValueKind.String)
        {
            return string.Equals(claim.GetString(), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        if (claim.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var item in claim.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.String &&
                string.Equals(item.GetString(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}).";
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
            {
                return detail.GetString() ?? "Request failed.";
            }

            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            {
                return title.GetString() ?? "Request failed.";
            }
        }
        catch
        {
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return "Invalid credentials.";
        }

        return content;
    }
}
