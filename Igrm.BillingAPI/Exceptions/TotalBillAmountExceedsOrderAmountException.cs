using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class TotalBillAmountExceedsOrderAmountException: ValidationException
    {
        public TotalBillAmountExceedsOrderAmountException(string? orderNumber = null) : base ($"Total bill amount exceeds order {orderNumber} amount")  {  }
    }
}
