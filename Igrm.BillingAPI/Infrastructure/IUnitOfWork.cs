using System.Threading.Tasks;

namespace Igrm.BillingAPI.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}