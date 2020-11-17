using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class NoBillsRegisteredException: ValidationException
    {
        public NoBillsRegisteredException(string? orderNumber=null):base($"No bills found for order {orderNumber}")
        {

        }
    }
}
