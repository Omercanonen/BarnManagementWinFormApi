using Business.DTOs;
using System.Net.Http.Json;

public sealed class ProductionApiService : IProductionApiService
{
    private readonly HttpClient _http;

    public ProductionApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AnimalProductionDto>> GetPotentialAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<AnimalProductionDto>>("api/Production/potential", ct)
               ?? new List<AnimalProductionDto>();
    }

    public async Task<List<AccumulatedProductDto>> GetPendingAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<AccumulatedProductDto>>("api/Production/pending", ct)
               ?? new List<AccumulatedProductDto>();
    }

    public async Task CollectAsync(List<AccumulatedProductDto> items, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Production/collect", items, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Collect Failed {error}");
        }
    }
}