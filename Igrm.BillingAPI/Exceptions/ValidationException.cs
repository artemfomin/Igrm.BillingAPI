using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Exceptions
{
    public abstract class ValidationException: Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}
