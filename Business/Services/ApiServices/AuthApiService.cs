using Business.Abstract.ApiServices;
using Business.DTOs.Auth;
using System.Net.Http.Json;

namespace Business.Services.ApiServices;

public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;

    public AuthApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("/api/Auth/login", dto, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Login failed ({(int)resp.StatusCode}): {body}");

        var data = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: ct);

        if (data is null || string.IsNullOrWhiteSpace(data.AccessToken))
            throw new InvalidOperationException("Login response is empty or token missing.");

        return data;
    }
}