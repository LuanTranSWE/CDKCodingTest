using DotnetCoding.Core.DTOs;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDetails>> GetAllProducts();
        Task<ProductDetails> GetProductById(int id);
        Task<IEnumerable<ProductDetails>> GetAllActiveProducts();
        Task<IEnumerable<ProductDetails>> GetProductsByNamePriceDate(string ProductName, int PriceFrom, int PriceTo, DateTime PostedDate);
        Task<ProductDetails> CreateProduct(ProductDetails productDetails);
        Task<ProductDetails> UpdateProductById(int id, ProductDetailsDTO? productDetailsDTO, string changeStatus = "");
        Task<string> DeleteProductById(int Id);
    }
}
