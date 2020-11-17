using Igrm.BillingAPI.Models.Business.Entities;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Commands
{
    public class AllocateSettlementCommandHandler : INotificationHandler<AllocateSettlementCommand>
    {
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger<AllocateSettlementCommandHandler> _logger;

        public AllocateSettlementCommandHandler(IOrderProcessingService orderProcessingService, ILogger<AllocateSettlementCommandHandler> logger)
        {
            _orderProcessingService = orderProcessingService;
            _logger = logger;
        }

        public async Task Handle(AllocateSettlementCommand request, CancellationToken cancellationToken)
        {
            if (request.Order is not null && (request.Order.OrderStatus == OrderStatus.Placed || request.Order.OrderStatus == OrderStatus.PartiallyPaid) )
            {
                try
                {
                    _logger.LogInformation($"Handling order {request.Order.OrderNumber} settlements allocations!");
                    await _orderProcessingService.AllocateSettlementsAsync(request.Order);
                    if ((request.Order.OrderStatus == OrderStatus.Paid || request.Order.OrderStatus == OrderStatus.Overpaid) && !String.IsNullOrEmpty(request.Order.Callback))
                    {
                        var receipt = await _orderProcessingService.GetReceiptAsync(request.Order);
                        await _orderProcessingService.ExecuteCallbackAsync(request.Order.Callback, receipt);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error occured while allocating settlements for order {request.Order.OrderNumber}: {ex.Message} {ex.StackTrace} {ex.InnerException}");
                }
            }
        }
    }
}
