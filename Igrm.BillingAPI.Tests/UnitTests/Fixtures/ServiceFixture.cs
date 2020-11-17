using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Services;
using Igrm.BillingAPI.Strategies;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Tests.UnitTests.Fixtures
{
    public class ServiceFixture : FixtureBase
    {
        public IGatewayStrategyFactory GatewayStrategyFactory { get; set; }
      

        public IOrderProcessingService OrderProcessingService { get; set; }


        public ServiceFixture() : base()
        {
            var fakeGatewayStrategyMock = new Mock<IGatewayStrategy>(MockBehavior.Strict);
            fakeGatewayStrategyMock.Setup(x => x.GetPaymentTo()).Returns("TEST-ACCOUNT");
            fakeGatewayStrategyMock.Setup(x => x.GetSettlementsAsync(It.IsAny<string>(), It.IsAny<string>()))
                                    .ReturnsAsync(new List<SettlementAllocationDTO>() {

                                        new SettlementAllocationDTO()
                                        {
                                            PaymentDate = DateTime.UtcNow,
                                            PaidAmount = 10,
                                            Description = String.Empty,
                                            PaymentFrom = "CUSTOMER-ACCOUNT",
                                            TxReference = "123"
                                        }

                                    });
            fakeGatewayStrategyMock.Verify();

            var gatewayStrategyFactoryMock = new Mock<IGatewayStrategyFactory>(MockBehavior.Strict);
            gatewayStrategyFactoryMock.Setup(x => x.CreateGatewayStrategy(It.IsAny<Gateway>())).Returns(fakeGatewayStrategyMock.Object);
            gatewayStrategyFactoryMock.Verify();
            GatewayStrategyFactory = gatewayStrategyFactoryMock.Object;

            const string testContent = "test content";
            var mockMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            mockMessageHandlerMock.Protected()
                                  .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                   .ReturnsAsync(new HttpResponseMessage
                                   {
                                       StatusCode = HttpStatusCode.OK,
                                       Content = new StringContent(testContent)
                                   });
            mockMessageHandlerMock.Verify();
            HttpClient = new HttpClient(mockMessageHandlerMock.Object);


            var loggerMock = new Mock<ILogger<IOrderProcessingService>>(MockBehavior.Loose);
            loggerMock.Verify();

            OrderProcessingService = new OrderProcessingService(UnitOfWork, OrderRepository, BillingConfigurationService, GatewayStrategyFactory, HttpClient, loggerMock.Object);
        }

    }
}
