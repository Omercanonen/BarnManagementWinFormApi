using Business.Abstract.ApiServices;
using Business.DTOs;
using System.Net.Http.Json;

namespace Business.Services.ApiServices
{
    public class WorkerApiService: IWorkerApiService
    {
        private readonly HttpClient _http;
        public WorkerApiService(HttpClient http) { _http = http; }

        public async Task<List<WorkerDto>> GetListAsync()
            => await _http.GetFromJsonAsync<List<WorkerDto>>("api/Worker/list") ?? new();

        public async Task BuyAsync()
        {
            var res = await _http.PostAsync("api/Worker/buy", null);
            if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
        }

        public async Task UpgradeAsync(int workerId)
        {
            var res = await _http.PostAsync($"api/Worker/upgrade/{workerId}", null);
            if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
        }

        public async Task SellAsync(int workerId)
        {
            var res = await _http.PostAsync($"api/Worker/sell/{workerId}", null);
            if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
        }
    }
}
