using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Igrm.BillingAPI.Exceptions;
using System.Linq;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Attributes;

namespace Igrm.BillingAPI.Models.Business.Entities
{
    public class Order
    {
        public Order()
        {
            Bills = new List<Bill>();
            Settlements = new List<Settlement>();
        }

        [Key]
        public int OrderId { get; set; }

        [Required]
        public string? OrderNumber { get; set; }

        public int UserId { get; set; }

        [Positive]
        public decimal OrderAmount { get; set; }

        [DefinedGateway]
        public Gateway Gateway { get; set; }

        public string? Description { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string? Callback { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public virtual ICollection<Bill> Bills { get; private set; }

        public virtual ICollection<Settlement> Settlements { get; private set; }

        public virtual Receipt? Receipt { get; private set; }

        public void AddBill(string paymentTo, decimal billAmount, DateTime dueDate)
        {
            if (OrderStatus != OrderStatus.Placed) { throw new OrderAlreadyProcessedException(OrderNumber); }

            var totalBilledAmount = Bills.Sum(x => x.BillAmount) + billAmount;

            if (totalBilledAmount > OrderAmount) { throw new TotalBillAmountExceedsOrderAmountException(OrderNumber); }

            if (string.IsNullOrEmpty(paymentTo)) { throw new СounterpartyBillingDetailRequiredException(OrderNumber); }

            if (billAmount <= 0) { throw new AmountLessThanOrEqualToZeroException("billAmount", OrderNumber); }

            Bills.Add(new Bill()
            {
                PaymentTo = paymentTo,
                BillAmount = billAmount,
                DueDate = dueDate,
                Created = DateTime.UtcNow,
                OrderId = OrderId
            });
        }

        public void AddSettlement(string? paymentFrom, decimal paidAmount, DateTime paymentDate, string? txReference = null, string? description = null)
        {
            if (string.IsNullOrEmpty(paymentFrom)) { throw new PayerDetailRequiredException(OrderNumber); }

            if (paidAmount <= 0) { throw new AmountLessThanOrEqualToZeroException("paidAmount", OrderNumber); }

            Settlements.Add(new Settlement()
            {
                PaymentFrom = paymentFrom,
                PaidAmount = paidAmount,
                PaymentDate = paymentDate,
                TxReference = txReference,
                Description = description,
                Created = DateTime.UtcNow,
                OrderId = OrderId
            });

            var totalPaidAmount = Settlements.Sum(x => x.PaidAmount);

            OrderStatus = (totalPaidAmount > OrderAmount) ? OrderStatus.Overpaid : (totalPaidAmount < OrderAmount) ? OrderStatus.PartiallyPaid : OrderStatus.Paid;

            if (OrderStatus == OrderStatus.Overpaid || OrderStatus == OrderStatus.Paid)
            {
                Receipt = new Receipt()
                {
                    ReceiptDate = DateTime.UtcNow,
                    Signature = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow,
                    OrderId = OrderId
                };
            }
        }

    }
}
