using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductQueueRepository : GenericRepository<ProductQueue>, IProductQueueRepository
    {
        protected readonly ProductDBContext _dbContext;
        public ProductQueueRepository(ProductDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

    }
}
