using System.ComponentModel.DataAnnotations;
using System;

namespace Igrm.BillingAPI.Models.Business.Entities
{
    public class Receipt
    {
        [Key]
        public int ReceiptId { get; set; }

        public DateTime ReceiptDate { get; set; }

        [Required]
        public string? Signature { get; set; }

        public DateTime Created { get; set; }

        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
