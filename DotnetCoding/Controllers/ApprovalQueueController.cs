using DotnetCoding.Core.DTOs;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApprovalQueueController : ControllerBase
    {
        public readonly IApprovalQueueService _approvalService;
        public readonly IProductService _productService;
        public ApprovalQueueController(IApprovalQueueService approvalQueueService, IProductService productService)
        {
            _approvalService = approvalQueueService;
            _productService = productService;
        }

        // Get Product from Approval Queue ONLY
        [HttpGet]
        public async Task<IActionResult> GetProductListFromApprovalQueue()
        {
            var productsOnApprovalQueue = await _approvalService.GetAllProductsOnApprovalQueue();
            if (productsOnApprovalQueue == null)
            {
                return NotFound();
            }
            return Ok(productsOnApprovalQueue);
        }

        // Get All Pending Approval Products on both ApprovalQueue and PendingProducts (Already included Product's name, Request Reason and Request Date)
        [HttpGet]
        public async Task<IActionResult> GetAllPendingApprovalProducts()
        {
            var allPendingProducts = await _approvalService.GetAllPendingApprovalProducts();

            if (allPendingProducts == null)
            {
                return NotFound();
            }
            
            return Ok(allPendingProducts);
        }

        // Approve Pending Product base on ID in ApprovalQueue Table
        // CASE 'CREATE' => Only update status from 'pending create' to 'active'
        // CASE 'UPDATE' => Update Product on Product table and update status from 'pending update' to 'active'
        // CASE 'DELETE' => Complete delete Product from Product table
        [HttpPost]
        public async Task<IActionResult> ApprovePendingProduct(string id)
        {
            var pendingProductById = await _approvalService.GetProductFromPendingProductById(id);
            var approvalQueueById = await _approvalService.GetProductFromApprovalQueueById(id);

            if (pendingProductById == null) 
            {
                return NotFound("Cannot your product. Please check again");
            }
   

            switch (approvalQueueById.State)
            {
                case "Create":
                    await _productService.UpdateProductById(approvalQueueById.ProductId, null, "active");
                    break;
                case "Update":
                    var approvedUpdateProduct = new ProductDetailsDTO
                    {
                        ProductName = pendingProductById.ProductName,
                        ProductStatus = "active",
                        ProductPrice = pendingProductById.ProductPrice,
                        PostedDate = pendingProductById.PostedDate,
                        ProductDescription = pendingProductById.ProductDescription
                    };
                    await _productService.UpdateProductById(approvalQueueById.ProductId, approvedUpdateProduct, "active");
                    break;
                case "Delete":
                    await _productService.DeleteProductById(approvalQueueById.ProductId);
                    break;
            }

            await _approvalService.DeleteProductFromPendingProductById(id);
            await _approvalService.DeleteProductFromApprovalQueueById(id);
            return Ok("Successfully Approved");
        }


        // Reject Pending Product base on ID in ApprovalQueue Table
        // Based on business logic it will be removed from ApprovalQueue and PendingProduct tables but remain on the Product Table
        [HttpPost]
        public async Task<IActionResult> RejectPendingProduct(string id)
        {
            // The following code is just my personal idea that we should mark status for rejected product as 'rejected' on Product table
            // Instead of keeping the status as 'pending delete'

            //var approvalQueueById = await _approvalService.GetProductFromApprovalQueueById(id);
            //await _productService.UpdateProductById(approvalQueueById.ProductId, null, "rejected");

            await _approvalService.DeleteProductFromPendingProductById(id);
            await _approvalService.DeleteProductFromApprovalQueueById(id);
            return Ok("Successfully Rejected");
        }
    }
}
