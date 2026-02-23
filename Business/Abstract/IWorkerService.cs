using Business.DTOs;

namespace Business.Abstract
{
    public interface IWorkerService
    {
        Task<List<WorkerDto>> GetWorkersAsync(int barnId);
        Task BuyWorkerAsync(int barnId, int userId);
        Task UpgradeWorkerAsync(int workerId, int userId);
        Task SellWorkerAsync(int workerId, int userId);
    }
}
