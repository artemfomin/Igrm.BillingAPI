using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Repositories;
using Igrm.BillingAPI.Services;
using Microsoft.Extensions.Logging;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Nethereum.HdWallet;
using Nethereum.Web3;
using System.Numerics;
using Numerics;

namespace Igrm.BillingAPI.Strategies
{
    public class EthereumGatewayStrategy : GatewayStrategyBase, IGatewayStrategy
    {
        private readonly Wallet _wallet;
        private readonly int _externalCallDelaySeconds;

        public EthereumGatewayStrategy(IBillingConfigurationService billingConfigurationService, ILogger logger, IOrderRepository orderRepository, IMemoryCache cache, HttpClient httpClient) : base(billingConfigurationService, logger, orderRepository, cache, httpClient)
        {
            _wallet = new Wallet(Convert.ToString(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Ethereum).Mnemonics),String.Empty);
            _externalCallDelaySeconds = Convert.ToInt32(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Ethereum).ExternalCallWaitSeconds) * 1000;
        }

        public string GetPaymentTo()
        {
            var noOfEthereumOrders = _orderRepository.GetMany(x => x.Gateway == Gateway.Ethereum).Count();
            return _wallet.GetAccount(noOfEthereumOrders).Address;
        }

        public async Task<ICollection<SettlementAllocationDTO>> GetSettlementsAsync(string? orderNumber, string paymentTo)
        {
            var result = new List<SettlementAllocationDTO>();
            await Task.Delay(_externalCallDelaySeconds);
            HttpResponseMessage response = await GetEtherTx(paymentTo);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var contentObject = JsonConvert.DeserializeObject<dynamic>(content);
                foreach (var tx in contentObject.result)
                {
                    var valueBigInteger = BigInteger.Parse(Convert.ToString(tx.value));
                    var valueBigRational = new BigRational(valueBigInteger) / new BigInteger(Math.Pow(10,18));
                    
                    result.Add(new SettlementAllocationDTO()
                    {
                        PaymentDate = Cave.UnixTime.UnixTime64.Convert(Convert.ToInt64(tx.timeStamp), DateTimeKind.Utc),
                        PaidAmount =  (Decimal)valueBigRational,
                        PaymentFrom = Convert.ToString(tx.from),
                        TxReference = Convert.ToString(tx.hash)
                    }) ;
                }
            }

            return result;
        }

        protected  virtual async Task<HttpResponseMessage> GetEtherTx(string paymentTo)
        {
            return await _httpClient?.GetAsync(Convert.ToString(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Ethereum).EtherScanAPIEndPoint).Replace("{address}", paymentTo));
        }
    }
}
