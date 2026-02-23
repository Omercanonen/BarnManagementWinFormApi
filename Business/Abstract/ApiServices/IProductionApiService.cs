using Business.DTOs;

public interface IProductionApiService
{
    Task<List<AnimalProductionDto>> GetPotentialAsync(CancellationToken ct = default);
    Task<List<AccumulatedProductDto>> GetPendingAsync(CancellationToken ct = default);
    Task CollectAsync(List<AccumulatedProductDto> items, CancellationToken ct = default);
}