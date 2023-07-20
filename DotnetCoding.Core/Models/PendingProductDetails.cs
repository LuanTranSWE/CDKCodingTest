
namespace DotnetCoding.Core.Models
{
    public class PendingProductDetails
    {
        public int Id { get; set; }
        public string ApprovalQueueId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public string ProductStatus { get; set; }
        public DateTime PostedDate { get; set; }
        public int? PreviousPrice { get; set; }
    }
}
