using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class OrderNumberNotProvidedException:ValidationException
    {
        public OrderNumberNotProvidedException() : base("Order number is empty or null")
        {

        }
    }
}
