using Igrm.BillingAPI.Models.Business.Values;
using System.ComponentModel.DataAnnotations;
using Igrm.BillingAPI.Attributes;
using System;

namespace Igrm.BillingAPI.Models.API
{
    public class PlaceOrderRequest
    {
        public PlaceOrderRequest()
        {
            OrderNumber = String.Empty;
        }

        [Required]
        public string? OrderNumber { get; set; }
        
        public int UserId { get; set; }
        
        [Positive]
        public decimal OrderAmount { get; set; }
        
        [DefinedGateway]
        public Gateway Gateway { get; set; }

        public string? Description { get; set; }

        public string? Callback { get; set; }
    }

}

