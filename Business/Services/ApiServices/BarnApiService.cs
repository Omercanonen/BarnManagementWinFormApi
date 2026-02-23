using Business.Abstract.ApiServices;
using Business.DTOs;
using Business.DTOs.Barn;
using System.Net.Http.Json;

namespace Business.Services.ApiServices;

public sealed class BarnApiService : IBarnApiService
{
    private readonly HttpClient _http;

    public BarnApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<MyBarnResponseDto> MyAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("/api/Barn/my", ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"MyBarn failed ({(int)res.StatusCode}): {body}");

        return (await res.Content.ReadFromJsonAsync<MyBarnResponseDto>(cancellationToken: ct))!;
    }

    public async Task<List<ActiveBarnListItemDto>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("api/Barn/all-active", ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"GetAllActive failed ({(int)res.StatusCode}): {body}");

        return (await res.Content.ReadFromJsonAsync<List<ActiveBarnListItemDto>>(cancellationToken: ct))!;
    }
}