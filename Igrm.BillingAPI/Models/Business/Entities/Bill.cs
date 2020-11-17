using System.ComponentModel.DataAnnotations;
using System;
using Igrm.BillingAPI.Models.DTO;

namespace Igrm.BillingAPI.Models.Business.Entities
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }
        [Required]
        public string? PaymentTo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal BillAmount { get; set; }

        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public DateTime Created { get; set; }

        public static explicit operator BillingInfoDTO(Bill bill)
        {
            return new BillingInfoDTO()
            {
                OrderNumber = bill.Order?.OrderNumber,
                BillAmount = bill.BillAmount,
                DueDate = bill.DueDate,
                PaymentTo = bill.PaymentTo
            };
        }
    }
}
