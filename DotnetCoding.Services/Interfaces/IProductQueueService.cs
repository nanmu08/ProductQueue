using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductQueueService
    {
        Task<IEnumerable<ProductQueue>> GetProductQueue();

        Task<ProductQueue> AddProductToQueueAsync(ProductDetail product, string QueueReason, string operation);

        Task<int> ApproveOrNotForOperationRequest(int ProductQueueId, int decision);

    }
}
