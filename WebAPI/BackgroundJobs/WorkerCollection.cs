using Business.Abstract;
using Business.DTOs;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.BackgroundJobs;

public class WorkerCollectionJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WorkerCollectionJob> _logger;

    public WorkerCollectionJob(IServiceScopeFactory scopeFactory, ILogger<WorkerCollectionJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productionService = scope.ServiceProvider.GetRequiredService<IProductionService>();

                var activeWorkers = await db.BarnWorkers
                    .Where(w => w.IsActive && w.BarnWorkerNextCollectionTime <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var worker in activeWorkers)
                {
                    var pendingProducts = productionService.GetAccumulatedProducts(worker.BarnId);

                    if (pendingProducts == null || !pendingProducts.Any())
                    {
                        UpdateNextCollectionTime(worker);
                        continue;
                    }


                    int currentLoad = 0;
                    int maxCapacity = worker.BarnWorkerCapacity;

                    var itemsToTake = new Dictionary<int, int>();

                    var tempStock = pendingProducts.ToDictionary(p => p.ProductId, p => p.TotalQuantity);

                    while (currentLoad < maxCapacity)
                    {
                        bool tookSomethingThisRound = false;

                        foreach (var product in pendingProducts)
                        {
                            if (currentLoad >= maxCapacity) break;

                            if (tempStock[product.ProductId] > 0)
                            {
                                if (!itemsToTake.ContainsKey(product.ProductId))
                                    itemsToTake[product.ProductId] = 0;

                                itemsToTake[product.ProductId]++;

                                tempStock[product.ProductId]--;

                                currentLoad++;

                                tookSomethingThisRound = true;
                            }
                        }

                       
                        if (!tookSomethingThisRound) break;
                    }
                    var finalCollectionList = new List<AccumulatedProductDto>();

                    foreach (var item in itemsToTake)
                    {
                        var originalProduct = pendingProducts.First(p => p.ProductId == item.Key);

                        finalCollectionList.Add(new AccumulatedProductDto
                        {
                            ProductId = item.Key,
                            ProductName = originalProduct.ProductName,
                            TotalQuantity = item.Value
                        });
                    }

                    if (finalCollectionList.Count > 0)
                    {
                        await productionService.CollectManualProductsAsync(worker.BarnId, finalCollectionList);
                        _logger.LogInformation($"Worker (Barn: {worker.BarnId}) ({maxCapacity}) {currentLoad} items collected");
                    }

                    UpdateNextCollectionTime(worker);
                }

                if (activeWorkers.Any())
                {
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker Error");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private void UpdateNextCollectionTime(Entities.Concrete.BarnWorker worker)
    {
        worker.BarnWorkerNextCollectionTime = DateTime.UtcNow.AddSeconds(worker.BarnWorkerIntervalSecond);
    }
}