using Business.DTOs;
using Business.DTOs.Barn;

namespace Business.Abstract.ApiServices
{
    public interface IBarnApiService
    {
        Task<MyBarnResponseDto> MyAsync(CancellationToken ct = default);
        Task<List<ActiveBarnListItemDto>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
