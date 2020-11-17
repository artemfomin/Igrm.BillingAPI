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
    public class GatewayStrategyBase
    {
        protected readonly IBillingConfigurationService _billingConfigurationService;
        protected readonly ILogger _logger;
        protected readonly IOrderRepository _orderRepository;
        protected readonly HttpClient? _httpClient;
        protected readonly IMemoryCache _cache;

        public GatewayStrategyBase(IBillingConfigurationService billingConfigurationService, ILogger logger, IOrderRepository orderRepository, IMemoryCache cache)
        {
            _billingConfigurationService = billingConfigurationService;
            _orderRepository = orderRepository;
            _logger = logger;
            _cache = cache;
            _orderRepository = orderRepository;
        }

        public GatewayStrategyBase(IBillingConfigurationService billingConfigurationService, ILogger logger, IOrderRepository orderRepository, IMemoryCache cache, HttpClient httpClient) : this( billingConfigurationService, logger, orderRepository, cache)
        {
            _httpClient = httpClient;
        }
    }
}
