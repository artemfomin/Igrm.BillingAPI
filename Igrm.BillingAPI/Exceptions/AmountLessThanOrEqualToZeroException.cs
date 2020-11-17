using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class AmountLessThanOrEqualToZeroException : ValidationException
    {
        public AmountLessThanOrEqualToZeroException(string columnName, string? orderNumber = null) : base($"{columnName} value is less than 0 for order {orderNumber}") { }
    }
}
