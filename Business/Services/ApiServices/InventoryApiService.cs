using Business.Abstract.ApiServices;
using Business.DTOs;
using System.Net.Http.Json;

namespace Business.Services.ApiServices
{
    public sealed class InventoryApiService : IInventoryApiService
    {
        private readonly HttpClient _http;

        public InventoryApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<InventoryItemDto>> GetInventoryAsync(CancellationToken ct = default)
        {
            return await _http.GetFromJsonAsync<List<InventoryItemDto>>("api/Inventory/list", ct)
                   ?? new List<InventoryItemDto>();
        }

        public async Task SellAsync(int productId, int quantity, CancellationToken ct = default)
        {
            var dto = new SellRequestDto
            {
                ProductId = productId,
                QuantityToSell = quantity
            };

            var res = await _http.PostAsJsonAsync("api/Inventory/sell", dto, ct);

            if (!res.IsSuccessStatusCode)
            {
                var error = await res.Content.ReadAsStringAsync(ct);
                throw new Exception(error);
            }
        }

        public async Task SellAllAsync(int productId, CancellationToken ct = default)
        {
            var res = await _http.PostAsJsonAsync("api/Inventory/sell-all", productId, ct);

            if (!res.IsSuccessStatusCode)
            {
                var error = await res.Content.ReadAsStringAsync(ct);
                throw new Exception(error);
            }
        }

        public async Task<string> ExportHistoryAsync(CancellationToken ct = default)
        {
            var res = await _http.GetAsync("api/Inventory/export-history", ct);

            if (!res.IsSuccessStatusCode)
                throw new Exception("History could'nt be downloaded");

            return await res.Content.ReadAsStringAsync(ct);
        }
    }
}