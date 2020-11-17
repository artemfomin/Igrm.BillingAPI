using System.Threading.Tasks;

namespace Igrm.BillingAPI.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private BillingAPIContext _dbContext;

        public UnitOfWork(BillingAPIContext context)
        {
            _dbContext = context;
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Commit()
        {
           _dbContext.SaveChanges();
        }
    }
}