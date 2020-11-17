using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class InvalidURLException : ValidationException
    {
        public InvalidURLException(string? orderNumber = null) : base($"Invalid callback url for order {orderNumber}. Please make sure you are using HTTPS protocol.") {  }
    }
}
