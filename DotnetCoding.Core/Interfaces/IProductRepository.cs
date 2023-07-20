using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<ProductDetails>
    {
        Task<IEnumerable<ProductDetails>> GetAllActiveProducts();
        Task<IEnumerable<ProductDetails>> GetProductsByNamePriceDate(string ProductName, int PriceFrom, int PriceTo, DateTime PostedDate);
    }
}
