using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<ProductDetails>, IProductRepository
    {
        public ProductRepository(DbContextClass dbContext) : base(dbContext)
        {

        }

        // Get All Active Products Order By Posted Date in Descending Order
        public async Task<IEnumerable<ProductDetails>> GetAllActiveProducts()
        {
            return await _dbContext.Set<ProductDetails>().Where(p => p.ProductStatus == "active").OrderByDescending(p => p.PostedDate).ToListAsync();
        }

        // Get Product Based On Name, Price Range and Posted Date
        public async Task<IEnumerable<ProductDetails>> GetProductsByNamePriceDate(string ProductName, int PriceFrom, int PriceTo, DateTime PostedDate)
        {
            return await _dbContext.Set<ProductDetails>().Where(p => p.ProductName == ProductName && p.ProductPrice >= PriceFrom && p.ProductPrice <= PriceTo && DateTime.Compare(p.PostedDate, PostedDate) == 0).ToListAsync();
        }
    }
}
