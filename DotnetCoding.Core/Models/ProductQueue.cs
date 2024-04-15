using System;
using System.Collections.Generic;

namespace DotnetCoding.Core.Models
{
    public partial class ProductQueue
    {
        public int Id { get; set; }
        public int? ProductDetailId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public int ProductPrice { get; set; }
        public int? ProductStatus { get; set; }
        public DateTime? ProductPostDate { get; set; }
        public DateTime? QueuePostDate { get; set; }
        public string QueueReason { get; set; } = null!;
        public string? QueueOperation { get; set; }
    }
}
