using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.DTO
{
    public class SettlementAllocationDTO
    {
        public string? PaymentFrom { get; set; }

        public decimal PaidAmount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? TxReference { get; set; }

        public string? Description { get; set; }
    }
}
