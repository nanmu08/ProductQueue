using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductDBContext _dbContext;
        public IProductRepository Products { get; }
        public IProductQueueRepository ProductQueues { get; }

        public UnitOfWork(ProductDBContext dbContext,
                            IProductRepository productRepository,
                            IProductQueueRepository productQueues)
        {
            _dbContext = dbContext;
            Products = productRepository;
            ProductQueues = productQueues;
        }

        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

    }
}
