using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class UnknownPaymentGatewayException : ValidationException
    {
        public UnknownPaymentGatewayException(int gateway): base($"Gateway {gateway} not recognized")
        {

        }
    }
}
