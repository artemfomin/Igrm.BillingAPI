using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class OrderAlreadyProcessedException : ValidationException
    {
        public OrderAlreadyProcessedException(string? orderNumber = null) : base($"Order {orderNumber} already processed!") { }
    }
}
