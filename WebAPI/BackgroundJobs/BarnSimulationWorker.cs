using Business.Abstract;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.BackgroundJobs;

public sealed class BarnSimulationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BarnSimulationWorker> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

    public BarnSimulationWorker(IServiceScopeFactory scopeFactory, ILogger<BarnSimulationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BarnSimulationWorker started. Interval: {Interval}", Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var aging = scope.ServiceProvider.GetRequiredService<IAgingService>();
                var production = scope.ServiceProvider.GetRequiredService<IProductionService>();

                var barnIds = await db.Barns
                    .Where(b => b.IsActive)
                    .Select(b => b.BarnId)
                    .ToListAsync(stoppingToken);

                foreach (var barnId in barnIds)
                {
                    try
                    {
                        await aging.ProcessAnimalGrowthAsync(barnId);
                        await production.ProduceAsync(barnId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Simulation tick failed for BarnId={BarnId}", barnId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BarnSimulationWorker loop error");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}