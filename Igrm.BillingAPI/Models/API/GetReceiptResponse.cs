using Igrm.BillingAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.API
{
    public class GetReceiptResponse
    {
        public ReceiptDTO? Receipt { get; set; }
    }
}
