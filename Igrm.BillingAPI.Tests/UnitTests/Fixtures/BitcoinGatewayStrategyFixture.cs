using Igrm.BillingAPI.Strategies;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Tests.UnitTests.Fixtures
{


    public class BitcoinGatewayStrategyFixture : FixtureBase
    {
        public BitcoinGatewayStrategy BitcoinGatewayStrategy { get; set; }

        public BitcoinGatewayStrategyFixture()
        {
            var bitcoinGatewayStrategyMock = new Mock<BitcoinGatewayStrategy>(BillingConfigurationService, FactoryLogger, OrderRepository, MemoryCache, HttpClient);
            
            bitcoinGatewayStrategyMock.Protected().Setup<Task<HttpResponseMessage>>("GetUnspentTxAsync", ItExpr.IsAny<string>())
                                                  .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK) 
                                                  { 
                                                      Content = new StringContent(@"{
  ""utxos"": [
    {
      ""txid"": ""78ffb00ae72702b0a37f7c2e85cc40caca7fde3086637f18d29e4a208e2bbfb5"",
      ""vout"": 0,
      ""amount"": 0.00008673,
      ""satoshis"": 8673,
      ""height"": 653632,
      ""confirmations"": 8313
    },
    {
      ""txid"": ""d5228d2cdc77fbe5a9aa79f19b0933b6802f9f0067f42847fc4fe343664723e5"",
      ""vout"": 0,
      ""amount"": 0.00006,
      ""satoshis"": 6000,
      ""height"": 629922,
      ""confirmations"": 32023
    }
  ],
  ""legacyAddress"": ""1Fg4r9iDrEkCcDmHTy2T79EusNfhyQpu7W"",
  ""cashAddress"": ""bitcoincash:qzs02v05l7qs5s24srqju498qu55dwuj0cx5ehjm2c"",
  ""slpAddress"": ""simpleledger:qzs02v05l7qs5s24srqju498qu55dwuj0c20jv8m5x"",
  ""scriptPubKey"": ""76a914a0f531f4ff810a415580c12e54a7072946bb927e88ac"",
  ""asm"": ""OP_DUP OP_HASH160 a0f531f4ff810a415580c12e54a7072946bb927e OP_EQUALVERIFY OP_CHECKSIG""
}", Encoding.UTF8, "application/json") 
                                                  }));
            
            bitcoinGatewayStrategyMock.Protected().Setup<Task<HttpResponseMessage>>("GetBlock", ItExpr.IsAny<string>())
                                                  .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                                                  {
                                                      Content = new StringContent(@"{
  ""hash"": ""000000004d78d2a8a93a1d20a24d721268690bebd2b51f7e80657d57e226eef9"",
  ""size"": 216,
  ""height"": 5000,
  ""version"": 1,
  ""merkleroot"": ""b0e585927e1737d07bd8157a2ba9f7615ef8ecd2af6d03523e51b4d23e134b6a"",
  ""tx"": [
    ""b0e585927e1737d07bd8157a2ba9f7615ef8ecd2af6d03523e51b4d23e134b6a""
  ],
  ""time"": 1235135895,
  ""nonce"": 3600108085,
  ""bits"": ""1d00ffff"",
  ""difficulty"": 1,
  ""chainwork"": ""0000000000000000000000000000000000000000000000000000138913891389"",
  ""confirmations"": 656946,
  ""previousblockhash"": ""00000000c9a61ea18fbf06b03e10033355e6eab3de038d975f40af9babbe0658"",
  ""nextblockhash"": ""00000000284bcd658fd7a76f5a88ee526f18592251341a05fd7f3d7abaf0c3ec"",
  ""reward"": 50,
  ""isMainChain"": true,
  ""poolInfo"": {}
}", Encoding.UTF8, "application/json")
                                                  }));
            
            bitcoinGatewayStrategyMock.Verify();
            BitcoinGatewayStrategy = bitcoinGatewayStrategyMock.Object;
        }
    }
}
