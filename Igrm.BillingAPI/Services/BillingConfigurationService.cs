using Igrm.BillingAPI.Models.Business.Values;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Services
{
    public interface IBillingConfigurationService
    {
        int GetDuePeriodDays();
        int GetDelay();
        int GetWarmUpPeriod();
        dynamic GetGatewaySpecificConfig(Gateway gateway);
    }

    public class BillingConfigurationService : IBillingConfigurationService
    {
        private readonly IConfiguration _configuration;

        public BillingConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int GetDelay()
        {
            return _configuration.GetSection("GeneralSetup")
                                 .GetValue<int>("AllocationWorkerWaitSeconds") * 1000;
        }

        public int GetDuePeriodDays()
        {
            return _configuration.GetSection("GeneralSetup")
                                 .GetValue<int>("DuePeriodDays");
        }

        public int GetWarmUpPeriod()
        {
            return _configuration.GetSection("GeneralSetup")
                                 .GetValue<int>("AllocationWorkerWarmUpWaitSeconds") * 1000;
        }

        public dynamic GetGatewaySpecificConfig(Gateway gateway)
        {
            var result = new ExpandoObject();

            foreach(var item in _configuration.GetSection(gateway.ToString()).GetChildren())
            {
                result.TryAdd(item.Key, (object)item.Get<dynamic>());
            }

            return result;
        }
    }
}
