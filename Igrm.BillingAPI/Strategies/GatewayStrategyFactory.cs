using Igrm.BillingAPI.Exceptions;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Repositories;
using Igrm.BillingAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Strategies
{
    public interface IGatewayStrategyFactory
    {
        IGatewayStrategy CreateGatewayStrategy(Gateway gateway);
    }

    public class GatewayStrategyFactory : IGatewayStrategyFactory
    {
        private readonly IBillingConfigurationService _billingConfigurationService;
        private readonly ILogger<IGatewayStrategyFactory> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public GatewayStrategyFactory(IBillingConfigurationService billingConfigurationService, ILogger<IGatewayStrategyFactory> logger, IOrderRepository orderRepository, IMemoryCache cache, HttpClient httpClient )
        {
            _billingConfigurationService = billingConfigurationService;
            _logger = logger;
            _orderRepository = orderRepository;
            _httpClient = httpClient;
            _cache = cache;
        }

        public IGatewayStrategy CreateGatewayStrategy(Gateway gateway)
        {
            return gateway switch
            {
                Gateway.SEPA => new SEPAGatewayStrategy(_billingConfigurationService, _logger, _orderRepository, _cache),
                Gateway.Bitcoin => new BitcoinGatewayStrategy(_billingConfigurationService, _logger, _orderRepository, _cache, _httpClient),
                Gateway.Ethereum => new EthereumGatewayStrategy(_billingConfigurationService, _logger, _orderRepository, _cache, _httpClient),
                _ => throw new UnknownPaymentGatewayException((int)gateway)
            };
        }
    }
}
