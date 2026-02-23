using Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract.ApiServices
{
    public interface IInventoryApiService
    {
        Task<List<InventoryItemDto>> GetInventoryAsync(CancellationToken ct = default);
        Task SellAsync(int productId, int quantity, CancellationToken ct = default);
        Task SellAllAsync(int productId, CancellationToken ct = default);
        Task<string> ExportHistoryAsync(CancellationToken ct = default);
    }
}
