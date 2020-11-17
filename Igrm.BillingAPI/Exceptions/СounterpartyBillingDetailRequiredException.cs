using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class СounterpartyBillingDetailRequiredException: ValidationException
    {
        public СounterpartyBillingDetailRequiredException(string? orderNumber = null):base($"Counterparty billing details not provided for order {orderNumber}")
        {

        }
    }
}
