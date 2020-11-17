using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class OrderNotFoundException: ValidationException
    {
        public OrderNotFoundException(string orderNumber) : base($"Order {orderNumber} not found!")  {   }
    }
}
