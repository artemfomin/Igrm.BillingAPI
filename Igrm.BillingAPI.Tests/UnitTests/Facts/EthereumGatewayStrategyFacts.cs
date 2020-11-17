using Igrm.BillingAPI.Tests.UnitTests.Fixtures;
using Xunit;

namespace Igrm.BillingAPI.Tests.UnitTests.Facts
{
    public class EthereumGatewayStrategyFacts
    {
        public class GetPaymentToMethod : IClassFixture<EthereumGatewayStrategyFixture>
        {
            private readonly EthereumGatewayStrategyFixture _fixture;
            public GetPaymentToMethod(EthereumGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public void WhenPaymentToRequested_ExpectBitcoinAccountAddressProvided()
            {
                var paymentTo = _fixture.EthereumGatewayStrategy.GetPaymentTo();
                Assert.Equal(42, paymentTo.Length);
            }
        }

        public class GetSettlementsMethod : IClassFixture<EthereumGatewayStrategyFixture>
        {
            private readonly EthereumGatewayStrategyFixture _fixture;

            public GetSettlementsMethod(EthereumGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenReadingSettlements_ExpectSettlementFound_Async()
            {
                var settlements = await _fixture.EthereumGatewayStrategy.GetSettlementsAsync("ORDER-123", _fixture.EthereumGatewayStrategy.GetPaymentTo());
                Assert.Equal(2, settlements.Count);
            }
        }
    }
}
