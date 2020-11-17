using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Igrm.BillingAPI.Models.DTO;

namespace Igrm.BillingAPI.Strategies
{
    public interface IGatewayStrategy
    {
        string GetPaymentTo();
        Task<ICollection<SettlementAllocationDTO>> GetSettlementsAsync(string? orderNumber, string paymentTo);
    }
}
