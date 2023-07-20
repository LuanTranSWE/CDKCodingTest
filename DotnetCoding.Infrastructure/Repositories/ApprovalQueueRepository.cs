using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ApprovalQueueRepository : GenericRepository<ApprovalQueue>, IApprovalQueueRepository
    {
        public ApprovalQueueRepository(DbContextClass context) : base(context)
        {
        }

        public async Task<IEnumerable<ApprovalQueue>> GetAllOrderByRequestDate()
        {
            return await _dbContext.Set<ApprovalQueue>().OrderBy(p => p.RequestDate).ToListAsync();
        }
    }
}
