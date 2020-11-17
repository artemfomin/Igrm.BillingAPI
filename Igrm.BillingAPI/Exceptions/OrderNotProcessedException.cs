using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class OrderNotProcessedException : ValidationException
    {
        public OrderNotProcessedException(string? orderNumber = null) : base($"Order {orderNumber} is not processed yet or is cancelled!") { }
    }
}
