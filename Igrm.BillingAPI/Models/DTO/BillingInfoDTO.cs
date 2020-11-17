using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.DTO
{
    public class BillingInfoDTO
    {
        public string? OrderNumber { get; set; }
        public string? PaymentTo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal BillAmount { get; set; }
    }
}
