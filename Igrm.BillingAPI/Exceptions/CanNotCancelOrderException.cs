using Igrm.BillingAPI.Models.Business.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public class CanNotCancelOrderException: ValidationException
    {
        public CanNotCancelOrderException(OrderStatus orderStatus, string? orderNumber=null):base($@"Order {orderNumber} status {orderStatus} is not status ""Placed"".")
        {

        }
    }
}
