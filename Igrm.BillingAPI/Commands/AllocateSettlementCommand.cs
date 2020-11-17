using Igrm.BillingAPI.Models.Business.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Commands
{
    public class AllocateSettlementCommand : INotification
    {
        public Order? Order { get; set; }
    }
}
