using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDetail>> GetAllProducts();

        Task<ProductDetail> GetProductById(int id);

        Task<int> AddProduct(ProductDetail product);

        Task<int> UpdateProduct(ProductDetail product);

        Task<int> DeleteProduct(int id);

        Task<IEnumerable<ProductDetail>> SearchProducts(string? ProductName, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate);
    }
}
