using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class OrderAlreadyPlacedException : ValidationException
    {
        public OrderAlreadyPlacedException(string? orderNumber = null) : base($"Order {orderNumber} already placed!") {  }
    }
}
