using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class PayerDetailRequiredException : ValidationException
    {
        public PayerDetailRequiredException(string? orderNumber = null) : base($"Payer not provided for order {orderNumber}")  { }
    }
}
