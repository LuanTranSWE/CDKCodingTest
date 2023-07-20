using DotnetCoding.Core.DTOs;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;
using System.Diagnostics.Metrics;

namespace DotnetCoding.Services
{
    public class ApprovalQueueService : IApprovalQueueService
    {
        public IUnitOfWork _unitOfWork;
        private static int counter = 0;
        public ApprovalQueueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // This method create a unique ID string. Logic is: Take the 64-bits and divide them in groups of 5 bits
        // (64 /5 is about 13 groups). Translate every 5 bits to one character => End up with 13 characters
        // This ID is used for ApprovalQueue 
        public string CreateUniqueID()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string UniqueId = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return UniqueId;
        }

        // this method create a unique ID integer. The logic is simpler => Get the tick, increase the counter and take addition of
        // timestamp and the increased unique integer.
        // This ID is used for ID in Product
        public int GenerateUniqueInteger()
        {
            int timestamp = (int)DateTime.UtcNow.Ticks;
            int uniqueInteger = Interlocked.Increment(ref counter);

            return timestamp + uniqueInteger;
        }

        /// Get the list of product in Approval Queue
        public async Task<IEnumerable<ApprovalQueue>> GetAllProductsOnApprovalQueue()
        {
            var productsOnApprovalQueue = await _unitOfWork.ApprovalQueue.GetAll();
            return productsOnApprovalQueue;
        }

        /// Add Product to Approval Queue and Pending Product 
        public async Task<ApprovalQueue> AddProductToApprovalQueue(ApprovalQueue productDetails, PendingProductDetails pendingProduct)
        {
            await _unitOfWork.ApprovalQueue.Create(productDetails);
            await _unitOfWork.PendingProduct.Create(pendingProduct);
            await _unitOfWork.SaveAsync();
            return productDetails;
        }

        /// Get All Pending Products
        public async Task<IEnumerable<PendingProductsDTO>> GetAllPendingApprovalProducts()
        {
            var productsOnApprovalQueue = await _unitOfWork.ApprovalQueue.GetAll();
            var pendingProducts = await _unitOfWork.PendingProduct.GetAll();

            // join 2 list on Approval Queue ID to get all pending approval products because we have 2 tables ApprovalQueue and PendingProduct
            var result = (from approval in productsOnApprovalQueue
                         join pending in pendingProducts on approval.Id equals pending.ApprovalQueueId
                         select new PendingProductsDTO()
                         {
                            RequestDate = approval.RequestDate,
                            RequestReason = approval.RequestReason,
                            State = approval.State,
                            ProductName = pending.ProductName,
                            ProductPrice = pending.ProductPrice,
                            ProductDescription = pending.ProductDescription,
                            ProductStatus = pending.ProductStatus,
                            PreviousPrice = pending.PreviousPrice,
                            PostedDate = pending.PostedDate
                         }).OrderBy(p => p.RequestDate);
            return result;
        }

        /// Get Pending Product From Approval Queue
        public async Task<ApprovalQueue> GetProductFromApprovalQueueById(string id)
        {
            var productById = await _unitOfWork.ApprovalQueue.SingleOrDefaultAsync(p => p.Id == id);
            return productById;
        }

        /// Get Pending Product From Pending Product
        public async Task<PendingProductDetails> GetProductFromPendingProductById(string id)
        {
            var productById = await _unitOfWork.PendingProduct.SingleOrDefaultAsync(p => p.ApprovalQueueId == id);
            return productById;
        }

        /// Delete Pending Product From Approval Queue By ID
        public async Task<string> DeleteProductFromApprovalQueueById(string id)
        {
            var productById = await GetProductFromApprovalQueueById(id);
            _unitOfWork.ApprovalQueue.Remove(productById);
            await _unitOfWork.SaveAsync();
            return "Delete Product From Approval Queue Successfully";
        }

        // Delete Pending Product From Pending Product Based On ID
        public async Task<string> DeleteProductFromPendingProductById(string id)
        {
            var productById = await GetProductFromPendingProductById(id);
            _unitOfWork.PendingProduct.Remove(productById);
            await _unitOfWork.SaveAsync();
            return "Delete Product From Pending Product Successfully";
        }
    }
}
