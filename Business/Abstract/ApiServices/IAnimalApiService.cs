using Business.DTOs;
using Business.DTOs.Barn;

namespace Business.Abstract.ApiServices
{
    public interface IAnimalApiService
    {
        Task<List<MyAnimalListItemDto>> GetMyAnimalsAsync(CancellationToken ct = default);

        Task<List<AnimalSpeciesDto>> GetSpeciesAsync(CancellationToken ct = default);
        Task PurchaseAnimalAsync(PurchaseAnimalDto dto, CancellationToken ct = default);
    }
}
