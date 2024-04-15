using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<ProductDetail>, IProductRepository
    {
        protected readonly ProductDBContext _dbContext;
        public ProductRepository(ProductDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        // 1.	Only active products should be listed in the page, the order should be latest first
        public async Task<IEnumerable<ProductDetail>> GetAllActiveProductAsync()
        {
           return await _dbContext.Set<ProductDetail>().Where(p => p.ProductStatus == 1).OrderByDescending(p => p.ProductPostDate).ToListAsync();
        }

        public async Task<IEnumerable<ProductDetail>> SearchProductsByQueryAsync(string? ProductName, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate)
        {
            if(ProductName == null)
            {
                return await _dbContext.Set<ProductDetail>().Where(p => 
                p.ProductPrice >= MinPrice && p.ProductPrice <= MaxPrice && p.ProductPostDate >= StartDate && p.ProductPostDate <= EndDate).ToListAsync();
            }
            else
            {
                return await _dbContext.Set<ProductDetail>().Where(p =>
                p.ProductName == ProductName &&
                p.ProductPrice >= MinPrice && p.ProductPrice <= MaxPrice &&
                p.ProductPostDate >= StartDate && p.ProductPostDate <= EndDate).ToListAsync();
            }
        }
    }
}
