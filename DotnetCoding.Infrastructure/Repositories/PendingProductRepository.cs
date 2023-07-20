using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class PendingProductRepository : GenericRepository<PendingProductDetails>, IPendingProductRepository
    {
        public PendingProductRepository(DbContextClass context) : base(context)
        {
        }
    }
}
