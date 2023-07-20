using DotnetCoding.Core.DTOs;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services
{
    public class ProductService : IProductService
    {
        public IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get All Products
        public async Task<IEnumerable<ProductDetails>> GetAllProducts()
        {
            var productDetailsList = await _unitOfWork.Products.GetAll();
            return productDetailsList;
        }

        // Get Product By ID
        public async Task<ProductDetails>GetProductById(int id)
        {
            var productById = await _unitOfWork.Products.SingleOrDefaultAsync(p => p.Id == id);
            return productById;
        }

        // Get All Active Products Order By Posted Date in Descending Order
        public async Task<IEnumerable<ProductDetails>> GetAllActiveProducts()
        {
            var productDetailsList = await _unitOfWork.Products.GetAllActiveProducts();
            return productDetailsList;
        }

        // Get Product Based On Name, Price Range and Posted Date
        public async Task<IEnumerable<ProductDetails>> GetProductsByNamePriceDate(string ProductName, int PriceFrom, int PriceTo, DateTime PostedDate)
        {
            var productDetailsList = await _unitOfWork.Products.GetProductsByNamePriceDate(ProductName, PriceFrom, PriceTo, PostedDate);
            return productDetailsList;
        }

        // Create Product and add to Product Table
        public async Task<ProductDetails> CreateProduct(ProductDetails productDetails)
        {
            await _unitOfWork.Products.Create(productDetails);
            await _unitOfWork.SaveAsync();
            return productDetails;
        }
        
        // Update Product By Id
        // There is a default parameter changeStatus in case we only need to update the status after approval
        public async Task<ProductDetails> UpdateProductById(int id, ProductDetailsDTO? productDetailsDTO, string changeStatus = "")
        {
            var productById = await GetProductById(id);

            if (productDetailsDTO == null)
            {
                productById.ProductStatus = changeStatus == "" ? productById.ProductStatus : changeStatus;
                await _unitOfWork.SaveAsync();
                return productById;
            }
            productById.ProductName = productDetailsDTO.ProductName == null ? productById.ProductName : productDetailsDTO.ProductName;
            productById.ProductStatus = productDetailsDTO.ProductStatus == null ? productById.ProductStatus : productDetailsDTO.ProductStatus;
            productById.ProductDescription = productDetailsDTO.ProductDescription == null ? productById.ProductDescription : productDetailsDTO.ProductDescription;
            productById.PostedDate = (DateTime)(!productDetailsDTO.PostedDate.HasValue ? productById.PostedDate : productDetailsDTO.PostedDate);
            if (!productDetailsDTO.ProductPrice.HasValue)
            {
                productById.ProductPrice = productById.ProductPrice;
            }
            else
            {
                // PreviousPrice is set to the current price and the current price is set to the new price
                productById.PreviousPrice = changeStatus == "active" ? productById.PreviousPrice : productById.ProductPrice;
                productById.ProductPrice = (int)productDetailsDTO.ProductPrice;
            }
            await _unitOfWork.SaveAsync();

            return productById;
        }

        // Delete Product By Id
        public async Task<string> DeleteProductById(int Id)
        {
            var productById = await GetProductById(Id);
            _unitOfWork.Products.Remove(productById);
            await _unitOfWork.SaveAsync();

            return "Product is pending for delete";
        }
    }
}
