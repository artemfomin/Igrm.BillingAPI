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

namespace Igrm.BillingAPI.Strategies
{
    public class BitcoinGatewayStrategy : GatewayStrategyBase, IGatewayStrategy
    {
        private readonly int _externalCallDelaySeconds;

        public BitcoinGatewayStrategy(IBillingConfigurationService billingConfigurationService, ILogger logger, IOrderRepository orderRepository, IMemoryCache cache, HttpClient httpClient) : base(billingConfigurationService, logger, orderRepository, cache, httpClient) 
        {
            _externalCallDelaySeconds = Convert.ToInt32(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Bitcoin).ExternalCallWaitSeconds) * 1000;
        }

        public string GetPaymentTo()
        {
            Key privateKey = new Key();
            BitcoinSecret bitcoinSecret = privateKey.GetWif(Network.Main);
            return bitcoinSecret.GetAddress(ScriptPubKeyType.Legacy).ToString();
        }

        public async Task<ICollection<SettlementAllocationDTO>> GetSettlementsAsync(string? orderNumber, string paymentTo)
        {
            var result = new List<SettlementAllocationDTO>();
            await Task.Delay(_externalCallDelaySeconds);
            HttpResponseMessage response = await GetUnspentTxAsync(paymentTo);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var contentObject = JsonConvert.DeserializeObject<dynamic>(content);
                foreach (var tx in contentObject.utxos)
                {
                    result.Add(new SettlementAllocationDTO()
                    {
                        PaymentDate = await GetBlockTimeByHeight(Convert.ToString(tx.height)),
                        PaidAmount = Convert.ToDecimal(tx.amount),
                        PaymentFrom = Convert.ToString(contentObject.legacyAddress),
                        TxReference = Convert.ToString(tx.txid)
                    });
                }
            }

            return result;
        }

        public async Task<DateTime> GetBlockTimeByHeight(string height)
        {
            return await _cache.GetOrCreateAsync<DateTime>(height, async (cacheEntry) =>
            {
                await Task.Delay(_externalCallDelaySeconds);
                HttpResponseMessage response = await GetBlock(height);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contentObject = JsonConvert.DeserializeObject<dynamic>(content);
                    return Cave.UnixTime.UnixTime64.Convert(Convert.ToInt64(contentObject.time), DateTimeKind.Utc);
                }
                else
                {
                    return DateTime.MinValue;
                }
            });

        }

        protected virtual async Task<HttpResponseMessage> GetUnspentTxAsync(string paymentTo)
        {
            return await _httpClient?.GetAsync(Convert.ToString(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Bitcoin).UnspentTXAPIEndPoint).Replace("{address}", paymentTo));
        }

        protected virtual async Task<HttpResponseMessage> GetBlock(string height)
        {
            return await _httpClient?.GetAsync(Convert.ToString(_billingConfigurationService.GetGatewaySpecificConfig(Gateway.Bitcoin).BlockAPIEndPoint).Replace("{height}", height));
        }

    }
}
