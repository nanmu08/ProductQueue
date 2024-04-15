using System;
using System.Collections.Generic;

namespace DotnetCoding.Core.Models
{
    public partial class ProductDetail
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public int ProductPrice { get; set; }
        public int? ProductStatus { get; set; }
        public DateTime? ProductPostDate { get; set; }
    }
}
