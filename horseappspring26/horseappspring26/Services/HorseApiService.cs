using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using horseappspring26.Models;
using Microsoft.Extensions.Logging;

namespace horseappspring26.Services;

public class HorseApiService(IHttpClientFactory httpClientFactory, ILogger<HorseApiService> logger) : IHorseApiService
{
    private const string HorseRoute = "api/Horse";

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedResponse<Horse>> GetHorsesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var route = $"{HorseRoute}?pageNumber={pageNumber}&pageSize={pageSize}";
        return await GetPagedAsync(route, cancellationToken);
    }

    public async Task<PagedResponse<Horse>> SearchHorsesAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var encoded = Uri.EscapeDataString(searchTerm);
        var route = $"{HorseRoute}/search?searchTerm={encoded}&pageNumber={pageNumber}&pageSize={pageSize}";
        return await GetPagedAsync(route, cancellationToken);
    }

    public async Task<Horse?> GetHorseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("HorseApi");
        var response = await client.GetAsync($"{HorseRoute}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<Horse>(_jsonOptions, cancellationToken);
    }

    public async Task<Horse> CreateHorseAsync(HorseUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("HorseApi");
        var response = await client.PostAsJsonAsync(HorseRoute, request, _jsonOptions, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);

        var createdHorse = await response.Content.ReadFromJsonAsync<Horse>(_jsonOptions, cancellationToken);
        return createdHorse ?? throw new ApiException("The API returned an empty response when creating the horse.");
    }

    public async Task<Horse> UpdateHorseAsync(int id, HorseUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("HorseApi");
        var response = await client.PutAsJsonAsync($"{HorseRoute}/{id}", request, _jsonOptions, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);

        var updatedHorse = await response.Content.ReadFromJsonAsync<Horse>(_jsonOptions, cancellationToken);
        return updatedHorse ?? throw new ApiException("The API returned an empty response when updating the horse.");
    }

    public async Task DeleteHorseAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("HorseApi");
        var response = await client.DeleteAsync($"{HorseRoute}/{id}", cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
    }

    private async Task<PagedResponse<Horse>> GetPagedAsync(string route, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("HorseApi");
        var response = await client.GetAsync(route, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<Horse>>(_jsonOptions, cancellationToken);
        return result ?? new PagedResponse<Horse>();
    }

    private async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var status = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = BuildErrorMessage(status, content);

        logger.LogWarning("Horse API call failed with {StatusCode}: {Message}", status, message);

        throw new ApiException(message);
    }

    private static string BuildErrorMessage(HttpStatusCode statusCode, string content)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                {
                    var detailValue = detail.GetString();
                    if (!string.IsNullOrWhiteSpace(detailValue))
                    {
                        return detailValue;
                    }
                }

                if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                {
                    var titleValue = title.GetString();
                    if (!string.IsNullOrWhiteSpace(titleValue))
                    {
                        return titleValue;
                    }
                }
            }
            catch
            {
            }

            return content;
        }

        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Request was invalid.",
            HttpStatusCode.Unauthorized => "You must log in before performing this action.",
            HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
            HttpStatusCode.NotFound => "The requested resource was not found.",
            _ => $"Request failed with status code {(int)statusCode} ({statusCode})."
        };
    }
}
