using Igrm.BillingAPI.Commands;
using Igrm.BillingAPI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Workers
{
    public class SettlementAllocationWorker : BackgroundService
    {
        private readonly ILogger<SettlementAllocationWorker> _logger;
        private readonly IServiceProvider _services;

        public SettlementAllocationWorker(IServiceProvider services, ILogger<SettlementAllocationWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Settlement allocation worker is running...");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
                var billingConfigurationService = scope.ServiceProvider.GetRequiredService<IBillingConfigurationService>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await Task.Delay(billingConfigurationService.GetWarmUpPeriod(), stoppingToken);
                _logger.LogInformation("Settlement allocation worker is working...");
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var ordersToProcess = await orderProcessingService.GetOrdersToProcessAsync();
                        _logger.LogInformation($"Settlement allocation worker is going to process {ordersToProcess.Count} orders...");
                        foreach (var order in ordersToProcess)
                        {
                            _logger.LogInformation($"Processing order {order.OrderNumber}");
                            await mediator.Publish(new AllocateSettlementCommand { Order = order });
                        }
                        await Task.Delay(billingConfigurationService.GetDelay(), stoppingToken);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"Exception occured wile processing orders: {ex.Message} {ex.StackTrace}");
                    }
                }
            }
        }
    }
}
