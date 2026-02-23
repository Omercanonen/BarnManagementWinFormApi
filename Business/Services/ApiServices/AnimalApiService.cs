using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs;
using Business.DTOs.Barn;
using System.Net.Http.Json;

namespace Business.Services.ApiServices;

public sealed class AnimalApiService : IAnimalApiService
{
    private readonly HttpClient _http;

    public AnimalApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MyAnimalListItemDto>> GetMyAnimalsAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("api/Animal/my", ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"{Messages.Error.AnimalListLoadError} ({(int)res.StatusCode}): {body}");

        return (await res.Content.ReadFromJsonAsync<List<MyAnimalListItemDto>>(cancellationToken: ct))!;
    }

    public async Task<List<AnimalSpeciesDto>> GetSpeciesAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("api/Animal/species", ct);

        if (!res.IsSuccessStatusCode)
            throw new Exception($"{Messages.Error.SpeciesLoadError}");

        return (await res.Content.ReadFromJsonAsync<List<AnimalSpeciesDto>>(cancellationToken: ct))!;
    }

    public async Task PurchaseAnimalAsync(PurchaseAnimalDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/Animal/purchase", dto, ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception(body);
        }
    }
}