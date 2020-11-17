using Igrm.BillingAPI.Infrastructure;
using Igrm.BillingAPI.Repositories;
using Igrm.BillingAPI.Services;
using Igrm.BillingAPI.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
using Moq;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Moq.Protected;
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;

namespace Igrm.BillingAPI.Tests.UnitTests.Fixtures
{
    public abstract class FixtureBase
    {
        public BillingAPIContext BillingAPIContext { get; set; }
        public IOrderRepository OrderRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IBillingConfigurationService BillingConfigurationService { get; set; }

        public ILogger<IGatewayStrategyFactory> FactoryLogger { get; set; }

        public IMemoryCache MemoryCache { get; set; }

        public HttpClient HttpClient { get; set; }


        public FixtureBase()
        {
            var options = SqliteInMemory.CreateOptions<BillingAPIContext>();
            BillingAPIContext = new BillingAPIContext(options);
            BillingAPIContext.Database.EnsureCreated();

            OrderRepository = new OrderRepository(BillingAPIContext);

            var billingConfigurationServiceMock = new Mock<IBillingConfigurationService>(MockBehavior.Strict);

            billingConfigurationServiceMock.Setup(x => x.GetDuePeriodDays()).Returns(10);
            billingConfigurationServiceMock.Setup(x => x.GetDelay()).Returns(1);
            billingConfigurationServiceMock.Setup(x => x.GetWarmUpPeriod()).Returns(10);
            
            var setup = new ExpandoObject();

            setup.TryAdd("ExternalCallWaitSeconds", 1);
            setup.TryAdd("UnspentTXAPIEndPoint", String.Empty);
            setup.TryAdd("BlockAPIEndPoint", String.Empty);
            setup.TryAdd("EtherScanAPIEndPoint", String.Empty);
            setup.TryAdd("XmlFilePath", "iso20022");
            setup.TryAdd("CompanyIBAN", "COMPANY-IBAN");
            setup.TryAdd("Mnemonics", "jealous expect hundred young unlock disagree major siren surge acoustic machine catalog");

            billingConfigurationServiceMock.Setup(x => x.GetGatewaySpecificConfig(It.IsAny<Gateway>())).Returns(setup);

            billingConfigurationServiceMock.Verify();

            BillingConfigurationService = billingConfigurationServiceMock.Object;

            UnitOfWork = new UnitOfWork(BillingAPIContext);

            var loggerMock = new Mock<ILogger<IGatewayStrategyFactory>>(MockBehavior.Loose);
            loggerMock.Verify();
            FactoryLogger = loggerMock.Object;

            MemoryCache = new MemoryCache(new MemoryCacheOptions());

            HttpClient = new HttpClient();
        }
    }
}
