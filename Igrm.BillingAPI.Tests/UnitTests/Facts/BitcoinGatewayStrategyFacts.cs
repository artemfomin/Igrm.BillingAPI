using Igrm.BillingAPI.Tests.UnitTests.Fixtures;
using Xunit;

namespace Igrm.BillingAPI.Tests.UnitTests.Facts
{
    public class BitcoinGatewayStrategyFacts
    {
        public class GetPaymentToMethod : IClassFixture<BitcoinGatewayStrategyFixture>
        {
            private readonly BitcoinGatewayStrategyFixture _fixture;
            
            public GetPaymentToMethod(BitcoinGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public void WhenPaymentToRequested_ExpectBitcoinLegacyAddressProvided()
            {
                var paymentTo = _fixture.BitcoinGatewayStrategy.GetPaymentTo();
                Assert.Equal(34, paymentTo.Length);
            }
        }

        public class GetSettlementsMethod : IClassFixture<BitcoinGatewayStrategyFixture>
        {
            private readonly BitcoinGatewayStrategyFixture _fixture;

            public GetSettlementsMethod(BitcoinGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenReadingSettlements_ExpectSettlementFound_Async()
            {
                var settlements = await _fixture.BitcoinGatewayStrategy.GetSettlementsAsync("ORDER-123", _fixture.BitcoinGatewayStrategy.GetPaymentTo());
                Assert.Equal(2, settlements.Count);
            }

        }
    }
}
