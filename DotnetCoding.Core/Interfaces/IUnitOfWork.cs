

namespace DotnetCoding.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IApprovalQueueRepository ApprovalQueue { get; }
        IPendingProductRepository PendingProduct { get; }

        int Save();
        Task<int> SaveAsync();
    }
}
