namespace DotnetCoding.Core.DTOs
{
    public class PendingProductsDTO
    {
        public DateTime RequestDate { get; set; }
        public string RequestReason { get; set; }
        public string State { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductStatus { get; set; }
        public int? PreviousPrice { get; set; }
        public DateTime PostedDate { get; set; }

    }
}
