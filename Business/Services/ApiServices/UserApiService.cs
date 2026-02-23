using Business.Abstract.ApiServices;
using Business.DTOs;
using System.Net.Http.Json;

namespace Business.Services.ApiServices;

public sealed class UserApiService : IUserApiService
{
    private readonly HttpClient _http;

    public UserApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserMeResponseDto> MeAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("/api/User/me", ct);

        var requestUri = res.RequestMessage?.RequestUri?.ToString();
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Me failed ({(int)res.StatusCode})\nRequestUri: {requestUri}\nBody: {body}");

        return (await res.Content.ReadFromJsonAsync<UserMeResponseDto>(cancellationToken: ct))!;
    }

}

