using Igrm.BillingAPI.Tests.UnitTests.Fixtures;
using Xunit;

namespace Igrm.BillingAPI.Tests.UnitTests.Facts
{
    public class SEPAGatewayStrategyFacts
    {

        public class GetPaymentToMethod : IClassFixture<SEPAGatewayStrategyFixture>
        {
            private readonly SEPAGatewayStrategyFixture _fixture;

            public GetPaymentToMethod(SEPAGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public void WhenPaymentToRequested_ExpectCompanyIBANProvided()
            {
                var paymentTo = _fixture.SEPAGatewayStrategy.GetPaymentTo();
                Assert.Equal("COMPANY-IBAN", paymentTo);
            }
        }

        public class GetSettlementsAsyncMethod : IClassFixture<SEPAGatewayStrategyFixture>
        {
            private readonly SEPAGatewayStrategyFixture _fixture;

            public GetSettlementsAsyncMethod(SEPAGatewayStrategyFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenReadingSettlements_ExpectSettlementFound_Async()
            {
                var settlements = await _fixture.SEPAGatewayStrategy.GetSettlementsAsync("ORDER-123", _fixture.SEPAGatewayStrategy.GetPaymentTo());
                Assert.Equal(1, settlements.Count);
            }
        }
    }
}
