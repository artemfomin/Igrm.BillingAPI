using System.ComponentModel.DataAnnotations;

namespace Igrm.BillingAPI.Models.API
{
    public class GetOrderStatusRequest
    {
        [Required]
        public string? OrderNumber { get; set; }
    }
}
