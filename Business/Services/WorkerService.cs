using Business.Abstract;
using Business.Constants;
using Business.DTOs;
using DataAccess.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly AppDbContext _context;
        private const decimal BaseWorkerPrice = 1000m; 

        public WorkerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkerDto>> GetWorkersAsync(int barnId)
        {
            var workers = await _context.BarnWorkers
                .Where(w => w.BarnId == barnId && w.IsActive)
                .ToListAsync();

            return workers.Select(w => new WorkerDto
            {
                Id = w.BarnWorkerId,
                Capacity = w.BarnWorkerCapacity,
                IntervalSeconds = w.BarnWorkerIntervalSecond,
                Level = w.BarnWorkerLevel,
                UpgradeCost = CalculateUpgradeCost(w.BarnWorkerLevel),
                SellPrice = CalculateSellPrice(w.BarnWorkerLevel),
                CanUpgrade = w.BarnWorkerIntervalSecond > 10
            }).ToList();
        }

        public async Task BuyWorkerAsync(int barnId, int userId)
        {
            var barn = await _context.Barns.FirstOrDefaultAsync(b => b.BarnId == barnId);
            if (barn == null) throw new Exception(Messages.Error.BarnNotFound);

            if (barn.BarnBalance < BaseWorkerPrice)
                throw new Exception($"{Messages.Error.InsufficientBalance}: {BaseWorkerPrice}");

            var worker = new BarnWorker
            {
                BarnId = barnId,
                BarnWorkerCapacity = 5,
                BarnWorkerIntervalSecond = 60,
                BarnWorkerLevel = 1,
                BarnWorkerNextCollectionTime = DateTime.UtcNow,
                IsActive = true
            };

            barn.BarnBalance -= BaseWorkerPrice;

            _context.BarnWorkers.Add(worker);
            _context.Barns.Update(barn);
            await _context.SaveChangesAsync();
        }

        public async Task UpgradeWorkerAsync(int workerId, int userId)
        {
            var worker = await _context.BarnWorkers.FindAsync(workerId);
            if (worker == null) throw new Exception("Worker not found");

            var barn = await _context.Barns.FindAsync(worker.BarnId);

            decimal cost = CalculateUpgradeCost(worker.BarnWorkerLevel);

            if (barn.BarnBalance < cost)
                throw new Exception(Messages.Error.InsufficientBalance);

            if (worker.BarnWorkerIntervalSecond <= 10)
                throw new Exception("This worker has reached maximum speed");

            worker.BarnWorkerLevel++;
            worker.BarnWorkerCapacity += 2;
            worker.BarnWorkerIntervalSecond = Math.Max(10, worker.BarnWorkerIntervalSecond - 5);

            barn.BarnBalance -= cost;

            _context.BarnWorkers.Update(worker);
            _context.Barns.Update(barn);
            await _context.SaveChangesAsync();
        }

        public async Task SellWorkerAsync(int workerId, int userId)
        {
            var worker = await _context.BarnWorkers.FindAsync(workerId);
            if (worker == null) return;

            var barn = await _context.Barns.FindAsync(worker.BarnId);

            decimal refund = CalculateSellPrice(worker.BarnWorkerLevel);
            barn.BarnBalance += refund;

            _context.BarnWorkers.Remove(worker);
            _context.Barns.Update(barn);
            await _context.SaveChangesAsync();
        }

        private decimal CalculateUpgradeCost(int level) => level * 500m;
        private decimal CalculateSellPrice(int level) => (level * 250m) + 200m;
    }
}

