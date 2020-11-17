using Igrm.BillingAPI.Strategies;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Tests.UnitTests.Fixtures
{
    public class EthereumGatewayStrategyFixture : FixtureBase
    {
        public EthereumGatewayStrategy EthereumGatewayStrategy { get; set; }

        public EthereumGatewayStrategyFixture()
        {
            var ethereumGatewayStrategyMock = new Mock<EthereumGatewayStrategy>(BillingConfigurationService, FactoryLogger, OrderRepository, MemoryCache, HttpClient);
            ethereumGatewayStrategyMock.Protected().Setup<Task<HttpResponseMessage>>("GetEtherTx", ItExpr.IsAny<string>())
                                        .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                                        {
                                            Content = new StringContent(@"{""status"":""1"",""message"":""OK"",""result"":[{""blockNumber"":""11055376"",""timeStamp"":""1602699058"",""hash"":""0x01b18866b2aa9daec1a9bb4f23fc2087918b92b324d7caba3c980dccc8172b92"",""nonce"":""1"",""blockHash"":""0x25d087c8704d7d404a8f559d9b1043061d0d6c548db5f97b6a5ad5ae387a4add"",""transactionIndex"":""141"",""from"":""0x758f30284d42075bb505f07232d098f5f24b86cd"",""to"":""0x2212d359cf1c5454ae949c5593a40e50a1fb10ba"",""value"":""4203250000000000"",""gas"":""21000"",""gasPrice"":""73000000000"",""isError"":""0"",""txreceipt_status"":""1"",""input"":""0x"",""contractAddress"":"""",""cumulativeGasUsed"":""7990958"",""gasUsed"":""21000"",""confirmations"":""220479""},{""blockNumber"":""11055378"",""timeStamp"":""1602699085"",""hash"":""0xb8ce8228805b748b7508d0a7e1ff1e36c07dd40c2905812acc5112f583974405"",""nonce"":""0"",""blockHash"":""0xcf72a19c5adf688e591e3da231bd0a4265be7482c4645fd9702d8d2365e72215"",""transactionIndex"":""82"",""from"":""0x2212d359cf1c5454ae949c5593a40e50a1fb10ba"",""to"":""0x931d387731bbbc988b312206c74f77d004d6b84b"",""value"":""2670250000000000"",""gas"":""21000"",""gasPrice"":""73000000000"",""isError"":""0"",""txreceipt_status"":""1"",""input"":""0x"",""contractAddress"":"""",""cumulativeGasUsed"":""6151866"",""gasUsed"":""21000"",""confirmations"":""220477""}]}")
                                        }));
            ethereumGatewayStrategyMock.Verify();
            EthereumGatewayStrategy = ethereumGatewayStrategyMock.Object;
        }


    }
}
