using System;
using System.Collections.Generic;
namespace DotnetCoding.Models
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public int ProductPrice { get; set; }
    }
}
