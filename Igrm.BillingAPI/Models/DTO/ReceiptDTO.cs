using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.DTO
{
    public class ReceiptDTO
    {
        public string? OrderNumber { get; set; }
        public string? Signature { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Debt { get; set; }
        public decimal Overpayment { get; set; }
    }
}
