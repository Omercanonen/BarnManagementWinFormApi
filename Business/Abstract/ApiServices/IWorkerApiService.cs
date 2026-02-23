using Business.DTOs;

namespace Business.Abstract.ApiServices
{
    public interface IWorkerApiService
    {
        Task<List<WorkerDto>> GetListAsync();
        Task BuyAsync();
        Task UpgradeAsync(int workerId);
        Task SellAsync(int workerId);
    }
}
