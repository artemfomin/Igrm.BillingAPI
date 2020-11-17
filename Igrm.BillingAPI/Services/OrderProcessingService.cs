using Igrm.BillingAPI.Exceptions;
using Igrm.BillingAPI.Infrastructure;
using Igrm.BillingAPI.Models.Business.Entities;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Repositories;
using Igrm.BillingAPI.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Services
{
    public interface IOrderProcessingService
    {
        Task<int> PlaceOrderAsync(string? orderNumber, int userId, decimal orderAmount, Gateway gateway, string? description = null, string? callback = null);
        Task AllocateSettlementsAsync(Order order);
        Task<ReceiptDTO> GetReceiptAsync(Order order);
        Task<Order> GetOrderAsync(string? orderNumber);
        Task<ICollection<Order>> GetOrdersToProcessAsync();
        Task CancelOrderAsync(Order order);
        Task ExecuteCallbackAsync(string? callback, ReceiptDTO receiptDto);
    }

    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IBillingConfigurationService _billingConfigurationService;
        private readonly IGatewayStrategyFactory _gatewayStrategyFactory;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IOrderProcessingService> _logger;

        public OrderProcessingService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IBillingConfigurationService billingConfigurationService,
                                      IGatewayStrategyFactory gatewayStrategyFactory, HttpClient httpClient, ILogger<IOrderProcessingService> logger)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _billingConfigurationService = billingConfigurationService;
            _gatewayStrategyFactory = gatewayStrategyFactory;
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task<Order> GetOrderAsync(string? orderNumber)
        {
            if (String.IsNullOrEmpty(orderNumber)) { throw new OrderNumberNotProvidedException(); }
            _logger.LogInformation($"Reading order {orderNumber} data.");
            var order = _orderRepository.Get(x => x.OrderNumber == orderNumber);
            if (order is null) throw new OrderNotFoundException(orderNumber);
            return Task.FromResult(order);
        }

        public async Task AllocateSettlementsAsync(Order order)
        {
            var billsPaymentTo = order.Bills.Select(x => x.PaymentTo).ToList().Distinct();
            if(!billsPaymentTo.Any())
            {
                throw new NoBillsRegisteredException(order.OrderNumber);
            }

            foreach (var billPaymentTo in billsPaymentTo)
            {
                if (!String.IsNullOrEmpty(billPaymentTo))
                {
                    var recognizedSettements = await _gatewayStrategyFactory.CreateGatewayStrategy(order.Gateway).GetSettlementsAsync(order.OrderNumber, billPaymentTo);
                    foreach(var settlement in recognizedSettements)
                    {
                        if(!order.Settlements.Where(x => x.TxReference == settlement.TxReference).Any())
                        {
                            _logger.LogInformation($"Allocating settlement to {order.OrderNumber} from {settlement.PaymentFrom}, tx reference {settlement.TxReference}.");
                            order.AddSettlement(settlement.PaymentFrom, settlement.PaidAmount, settlement.PaymentDate, settlement.TxReference, settlement.Description);
                        }
                    }
                }
                else
                {
                    throw new СounterpartyBillingDetailRequiredException(order.OrderNumber);
                }
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task ExecuteCallbackAsync(string? callback, ReceiptDTO receiptDto)
        {
            if (!String.IsNullOrEmpty(callback))
            {
                _logger.LogInformation($"Calling back {callback} with receipt data for order {receiptDto.OrderNumber}");
                await _httpClient.PostAsJsonAsync(callback, receiptDto);
            }
        }

        public async Task<ReceiptDTO> GetReceiptAsync(Order order)
        {
            _logger.LogInformation($"Getting receipt for order {order.OrderNumber}");
            if (order.OrderStatus == OrderStatus.Paid || order.OrderStatus == OrderStatus.Overpaid || order.OrderStatus == OrderStatus.PartiallyPaid)
            {
                var diff = order.Settlements.Sum(x => x.PaidAmount) - order.OrderAmount;
                var result = new ReceiptDTO()
                {
                    OrderNumber = order.OrderNumber,
                    Signature = order.Receipt?.Signature,
                    PaymentDate = order.Settlements.Max(x => x.PaymentDate),
                    Amount = order.OrderAmount,
                    Overpayment = diff > 0 ? diff : 0,
                    Debt = diff < 0 ? Math.Abs(diff) : 0
                };
                return await Task.FromResult(result);
            }
            throw new OrderNotProcessedException(order.OrderNumber);
        }

        public async Task<int> PlaceOrderAsync(string? orderNumber, int userId, decimal orderAmount, Gateway gateway, string? description = null, string? callback = null)
        {
            _logger.LogInformation($"Placing order {orderNumber}");

            if (orderAmount <= 0) { throw new AmountLessThanOrEqualToZeroException("orderAmount", orderNumber); }
            if (!Enum.IsDefined(gateway)) { throw new UnknownPaymentGatewayException((int)gateway); }
            if (String.IsNullOrEmpty(orderNumber)) { throw new OrderNumberNotProvidedException(); }
            if (_orderRepository.Get(x => x.OrderNumber == orderNumber) is not null) { throw new OrderAlreadyPlacedException(orderNumber); }
            if (!String.IsNullOrEmpty(callback) && !(Uri.TryCreate(callback, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttps)) { throw new InvalidURLException(orderNumber); }

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userId,
                OrderAmount = orderAmount,
                Gateway = gateway,
                Description = description,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                OrderStatus = OrderStatus.Placed,
                Callback = callback
            };
            var paymentTo = _gatewayStrategyFactory.CreateGatewayStrategy(gateway).GetPaymentTo();

            order.AddBill(paymentTo, orderAmount, DateTime.UtcNow.AddDays(_billingConfigurationService.GetDuePeriodDays()));

            _orderRepository.Add(order);

            await _unitOfWork.CommitAsync();

            return order.OrderId;
        }

        public async Task<ICollection<Order>> GetOrdersToProcessAsync()
        {
            return await Task.FromResult(_orderRepository
                                           .GetMany(x => x.OrderStatus == OrderStatus.Placed 
                                                    || x.OrderStatus == OrderStatus.PartiallyPaid)
                                           .ToList());
        }

        private async Task UpdateOrderAsync(Order order)
        {
            _logger.LogInformation($"Updating order {order.OrderNumber}");
            _orderRepository.Update(order);
            await _unitOfWork.CommitAsync();
        }

        public async Task CancelOrderAsync(Order order)
        {
            if(order.OrderStatus == OrderStatus.Placed)
            {
                order.OrderStatus = OrderStatus.Canceled;
                await UpdateOrderAsync(order);
            }
            else
            {
                throw new CanNotCancelOrderException(order.OrderStatus, order.OrderNumber);
            }
            
        }
    }
}
