using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.Business.Values
{
    public enum OrderStatus
    {
        Placed = 1, Paid = 2, PartiallyPaid = 3, Overpaid = 4, Returned = 5, Canceled = 6
    }
}
