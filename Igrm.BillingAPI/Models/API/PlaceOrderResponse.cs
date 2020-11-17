using Igrm.BillingAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Models.API
{
    public class PlaceOrderResponse
    {
        public PlaceOrderResponse()
        {
            Bills = new List<BillingInfoDTO>();
        }

        public List<BillingInfoDTO> Bills { get; set; }
    }
}
