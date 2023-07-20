using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.Core.DTOs;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IProductService _productService;
        public readonly IApprovalQueueService _approvalQueueService;
        public ProductsController(IProductService productService, IApprovalQueueService approvalQueueService)
        {
            _productService = productService;
            _approvalQueueService = approvalQueueService;
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var productDetailsList = await _productService.GetAllProducts();
            if(productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }


        // Get All Active Products
        [HttpGet]
        public async Task<IActionResult> GetActiveProductList()
        {
            var productDetailsList = await _productService.GetAllActiveProducts();
            if (productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }

        // Get Product Based On Name, Price Range and Posted Date
        [HttpGet]
        public async Task<IActionResult> GetProductsByNamePriceDate(string ProductName, int PriceFrom, int PriceTo, string PostedDate)
        {
            DateTime PostedDateParse = DateTime.Parse(PostedDate);
            var productDetailsList = await _productService.GetProductsByNamePriceDate(ProductName, PriceFrom, PriceTo, PostedDateParse);
            if (productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }

        // Create new Product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDetailsDTO productDetailsDTO)
        {
            if (productDetailsDTO.ProductName == null || productDetailsDTO.ProductStatus == null || !productDetailsDTO.PostedDate.HasValue || !productDetailsDTO.ProductPrice.HasValue)
            {
                return BadRequest();
            }
            // If the new Product's price exceed $10000 => Abort the creation and return
            if (productDetailsDTO.ProductPrice > 10000)
            {
                return BadRequest("Product creation is not allowed because its price is more than 10000 dollars");
            }
            int ID = _approvalQueueService.GenerateUniqueInteger();
            var newProduct = new ProductDetails
            {
                Id = ID,
                ProductName = productDetailsDTO.ProductName,
                ProductStatus = "active",
                ProductDescription = productDetailsDTO.ProductDescription!, // use null forgiving operator because ProductDescription may be null
                ProductPrice = (int)productDetailsDTO.ProductPrice,
                PostedDate = (DateTime)productDetailsDTO.PostedDate,
            };
            // If the new Product's price exceed $5000 => Add to Product and mark status as 'pending create' and Add to ApprovalQueue
            // and PendingProducts tables.
            if (productDetailsDTO.ProductPrice > 5000)
            {
                string UniqueId = _approvalQueueService.CreateUniqueID();
                var approvalQueue = new ApprovalQueue
                {
                    Id = UniqueId,
                    RequestReason = $"Product creation price { productDetailsDTO.ProductPrice } more than 5000 dollars.",
                    RequestDate = DateTime.Now,
                    ProductId = ID,
                    State = "Create"
                };
                var pendingProduct = new PendingProductDetails
                {
                    ApprovalQueueId = UniqueId,
                    ProductName = productDetailsDTO.ProductName,
                    ProductStatus = "pending create",
                    ProductPrice = (int)productDetailsDTO.ProductPrice,
                    PostedDate = (DateTime)productDetailsDTO.PostedDate,
                    ProductDescription = productDetailsDTO.ProductDescription! 
                };

                newProduct.ProductStatus = "pending create";
                await _productService.CreateProduct(newProduct);
                await _approvalQueueService.AddProductToApprovalQueue(approvalQueue, pendingProduct);
                return Ok("Product is added on Approval Queue");
            }
            // If the price is not in the above conditions => Only create product directly in database and mark status as 'active'
            await _productService.CreateProduct(newProduct);
            return Ok("Product is created on the database");
        }


        // Update Product based on ID
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDetailsDTO productDetailsDTO)
        {
            var productDetails = await _productService.GetProductById(id);
            if (productDetails == null)
            {
                return NotFound("Cannot find your product");
            }

            // If the price is updated and it's > 5000 or larger than 50% of its previous price
            // Then change the status in Product table to 'pending update' and Add to ApprovalQueue and PendingProducts tables.
            if (productDetailsDTO != null && (productDetailsDTO.ProductPrice > 1.5 * productDetails.ProductPrice ||
                productDetailsDTO.ProductPrice > 5000))
            {
                string UniqueId = _approvalQueueService.CreateUniqueID();
                var approvalQueue = new ApprovalQueue
                {
                    Id = UniqueId,
                    ProductId = id,
                    RequestReason = $"Product Update price {productDetailsDTO.ProductPrice} more than 5000 dollars or your update price is larger than 50% compare to previous price",
                    RequestDate = DateTime.Now,
                    State = "Update"
                };
                var pendingProduct = new PendingProductDetails
                {
                    ApprovalQueueId = UniqueId,
                    ProductName = productDetailsDTO.ProductName == null ? productDetails.ProductName : productDetailsDTO.ProductName,
                    ProductStatus = "pending update",
                    PreviousPrice = productDetails.ProductPrice,
                    ProductPrice = (int)productDetailsDTO.ProductPrice,
                    PostedDate = (DateTime)(!productDetailsDTO.PostedDate.HasValue ? productDetails.PostedDate : productDetailsDTO.PostedDate),
                    ProductDescription = productDetailsDTO.ProductDescription == null ? productDetails.ProductDescription : productDetailsDTO.ProductDescription
                };
                productDetailsDTO.ProductStatus = "pending update";
                await _productService.UpdateProductById(id, productDetailsDTO);
                await _approvalQueueService.AddProductToApprovalQueue(approvalQueue, pendingProduct);
                return Ok("Product is added on Approval Queue");
            }

            // If the price is not in the above conditions => Update product directly in database and mark status as 'active'
            var updatedProduct = await _productService.UpdateProductById(id, productDetailsDTO);
            if (updatedProduct == null)
            {
                return BadRequest("Problem with updating product. Please check again");
            }
            return Ok(updatedProduct);
        }


        // Delete Product by ID
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productDetails = await _productService.GetProductById(id);
            if (productDetails == null)
            {
                return NotFound("Cannot find your product");
            }
            string UniqueId = _approvalQueueService.CreateUniqueID();
            var approvalQueue = new ApprovalQueue
            {
                Id = UniqueId,
                ProductId = id,
                RequestReason = $"Delete this product",
                RequestDate = DateTime.Now,
                State = "Delete"
            };
            var pendingProduct = new PendingProductDetails
            {
                ApprovalQueueId = UniqueId,
                ProductName = productDetails.ProductName,
                ProductStatus = "pending delete",
                PreviousPrice = productDetails.PreviousPrice,
                ProductPrice = productDetails.ProductPrice,
                PostedDate =  productDetails.PostedDate,
                ProductDescription = productDetails.ProductDescription
            };
            // According to business logic => Don't delete immediately => Add to ApprovalQueue and PendingProduct table and mark
            // 'pending delete' on Product table
            await _approvalQueueService.AddProductToApprovalQueue(approvalQueue, pendingProduct);
            await _productService.UpdateProductById(id, null, "pending delete");
            return Ok("Product is pending approval for deletion");
        }
    }
}
