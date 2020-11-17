using System.ComponentModel.DataAnnotations;

namespace Igrm.BillingAPI.Models.API
{
    public class CancelOrderRequest
    {
        [Required]
        public string? OrderNumber { get; set; }
    }
}
