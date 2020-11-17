using Igrm.BillingAPI.Infrastructure;
using Igrm.BillingAPI.Models.Business.Entities;

namespace Igrm.BillingAPI.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
    }

    public class OrderRepository: RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(BillingAPIContext context) : base(context) { }
    }
}
