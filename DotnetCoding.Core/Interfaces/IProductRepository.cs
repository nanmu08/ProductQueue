using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<ProductDetail>
    {
        Task<IEnumerable<ProductDetail>> GetAllActiveProductAsync();

        Task<IEnumerable<ProductDetail>> SearchProductsByQueryAsync(string? ProductName, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate);
    }
}
