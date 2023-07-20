using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces
{
    public interface IApprovalQueueRepository : IGenericRepository<ApprovalQueue>
    {
        Task<IEnumerable<ApprovalQueue>> GetAllOrderByRequestDate();
    }
}
