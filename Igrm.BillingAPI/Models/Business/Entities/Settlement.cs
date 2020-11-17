using System.ComponentModel.DataAnnotations;
using System;

namespace Igrm.BillingAPI.Models.Business.Entities
{
    public class Settlement
    {
        [Key]
        public int SettlementId { get; set; }
        
        [Required]
        public string? PaymentFrom { get; set; }
        
        public decimal PaidAmount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? TxReference { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
