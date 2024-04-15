using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services
{
    public class ProductQueueService : IProductQueueService
    {
        public IUnitOfWork _unitOfWork;
        public ProductQueueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductQueue>> GetProductQueue()
        {
            var ProductQueues = await _unitOfWork.ProductQueues.GetAllAsync();
            ProductQueues = ProductQueues.OrderBy(x => x.QueuePostDate).ToList();
            return ProductQueues;
        }

        public async Task<ProductQueue> AddProductToQueueAsync(ProductDetail product,  string QueueReason, string operation)
        {
            ProductQueue ProductToBeAdded = new ProductQueue();
            ProductToBeAdded.ProductDetailId = product.Id;
            ProductToBeAdded.ProductName = product.ProductName;
            ProductToBeAdded.ProductDescription = product.ProductDescription;
            ProductToBeAdded.ProductPrice = product.ProductPrice;
            ProductToBeAdded.ProductStatus = product.ProductStatus;
            ProductToBeAdded.ProductPostDate = product.ProductPostDate;
            ProductToBeAdded.QueuePostDate = DateTime.Now;
            ProductToBeAdded.QueueReason = QueueReason;
            ProductToBeAdded.QueueOperation = operation;

            await _unitOfWork.ProductQueues.AddAsync(ProductToBeAdded);
            await _unitOfWork.Save();
            return ProductToBeAdded;
        }

        public async Task<int> ApproveOrNotForOperationRequest(int ProductQueueId, int decision)
        {
            var productFromQueue = await _unitOfWork.ProductQueues.GetByIdAsync(ProductQueueId);
            if(productFromQueue == null)
            {
                return 0;
            }
            else
            {
                // if decision is Accept(1), then update new state to product detail table and remove it from queue
                if (decision == 1)
                {

                    if (productFromQueue.QueueOperation == "CREATE")
                    {
                        ProductDetail productToBeHandled = new ProductDetail();
                        productToBeHandled.Id = productFromQueue.ProductDetailId ?? 0;
                        productToBeHandled.ProductName = productFromQueue.ProductName;
                        productToBeHandled.ProductDescription = productFromQueue.ProductDescription;
                        productToBeHandled.ProductPrice = productFromQueue.ProductPrice;
                        productToBeHandled.ProductStatus = 1;
                        productToBeHandled.ProductPostDate = productFromQueue.ProductPostDate;
                        await _unitOfWork.Products.AddAsync(productToBeHandled);
                        await _unitOfWork.Save();
                    }
                    else if (productFromQueue.QueueOperation == "UPDATE")
                    {
                        var productToBeUpdated = await _unitOfWork.Products.GetByIdAsync((int)productFromQueue.ProductDetailId);
                        productToBeUpdated.ProductName = productFromQueue.ProductName;
                        productToBeUpdated.ProductDescription = productFromQueue.ProductDescription;
                        productToBeUpdated.ProductPrice = productFromQueue.ProductPrice;
                        productToBeUpdated.ProductStatus = 1;
                        productToBeUpdated.ProductPostDate = productFromQueue.ProductPostDate;
                        await _unitOfWork.Save();
                    }
                    else if (productFromQueue.QueueOperation == "DELETE")
                    {
                        var productToBeUpdated = await _unitOfWork.Products.GetByIdAsync((int)productFromQueue.ProductDetailId);
                        _unitOfWork.Products.RemoveAsync(productToBeUpdated);
                        await _unitOfWork.Save();
                    }

                    // remove product from queue
                    _unitOfWork.ProductQueues.RemoveAsync(productFromQueue);
                    await _unitOfWork.Save();
                }
                else
                {
                    // if decision is Refuse (0), update keep origin data in product detail table, change status to 1, then remove it from queue
                    // for create operation in queue, because it has not in product table, so no need to update it just remove it from queue
                    if (productFromQueue.QueueOperation == "UPDATE" || productFromQueue.QueueOperation == "DELETE")
                    {
                        var productToBeUpdated = await _unitOfWork.Products.GetByIdAsync((int)productFromQueue.ProductDetailId);
                        productToBeUpdated.ProductStatus = 1;
                        await _unitOfWork.Save();
                    }
                    // then remove product from queue
                    _unitOfWork.ProductQueues.RemoveAsync(productFromQueue);
                    await _unitOfWork.Save();

                }

                return 1;
            }
            
        }
    }
}
