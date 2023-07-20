using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetCoding.Core.Models
{
    public class ApprovalQueue
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string RequestReason { get; set; }
        public DateTime RequestDate { get; set; }
        public string State { get; set; }
    }
}
