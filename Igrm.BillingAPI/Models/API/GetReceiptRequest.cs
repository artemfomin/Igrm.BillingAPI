using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.API
{
    public class GetReceiptRequest
    {
        [Required]
        public string? OrderNumber { get; set; }
    }
}
