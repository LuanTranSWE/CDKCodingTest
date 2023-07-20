using DotnetCoding.Core.DTOs;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IApprovalQueueService
    {
        Task<IEnumerable<ApprovalQueue>> GetAllProductsOnApprovalQueue();
        Task<ApprovalQueue> AddProductToApprovalQueue(ApprovalQueue productDetails, PendingProductDetails pendingProduct);
        Task<IEnumerable<PendingProductsDTO>> GetAllPendingApprovalProducts();
        Task<ApprovalQueue> GetProductFromApprovalQueueById(string id);
        Task<PendingProductDetails> GetProductFromPendingProductById(string id);
        Task<string> DeleteProductFromApprovalQueueById(string id);
        Task<string> DeleteProductFromPendingProductById(string id);
        string CreateUniqueID();
        int GenerateUniqueInteger();
    }
}
