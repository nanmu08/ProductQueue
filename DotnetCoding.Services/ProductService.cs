using System;
using System.Collections;
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
    public class ProductService : IProductService
    {
        public IUnitOfWork _unitOfWork;
        public readonly IProductQueueService _productQueueService;

        public ProductService(IUnitOfWork unitOfWork, IProductQueueService productQueueService)
        {
            _unitOfWork = unitOfWork;
            _productQueueService = productQueueService;
        }

        public async Task<IEnumerable<ProductDetail>> GetAllProducts()
        {
            // 1.	Only active products should be listed in the page, the order should be latest first
            var productDetailsList = await _unitOfWork.Products.GetAllActiveProductAsync();
            return productDetailsList;
        }

        public async Task<ProductDetail> GetProductById(int id)
        {
            var productDetail = await _unitOfWork.Products.GetByIdAsync(id);
            return productDetail;
        }

        public async Task<int> AddProduct(ProductDetail product)
        {
            product.ProductStatus = 1;
            product.ProductPostDate = DateTime.Now;
            //4.	Any product should be pushed to approval queue if its price is more than 5000 dollars at the time of creation and update.
            if(product.ProductPrice > 5000)
            {
                await _productQueueService.AddProductToQueueAsync(product, "price more than 5000 at the time of create", "CREATE");
                return 2;
            }
            else
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.Save();
                return 1;
            }


        }

        public async Task<int> UpdateProduct(ProductDetail product)
        {
            var productToBeUpdated = await _unitOfWork.Products.GetByIdAsync(product.Id);

            // assign status and postdate stored in db to product object
            product.ProductStatus = productToBeUpdated.ProductStatus;
            product.ProductPostDate = productToBeUpdated.ProductPostDate;

            // firstly check product status, if status is 0, then will refuce this operation cause this product may be waiting for approval in queue
            if(productToBeUpdated.ProductStatus == 0)
            {
                // "5" means this request cannot be handled now cause status is inactive
                return 5;
            }

            if(product.ProductPrice > 5000)
            {
                //4.Any product should be pushed to approval queue if its price is more than 5000 dollars at the time of creation and update.
                // set product detail data status with inactive
                productToBeUpdated.ProductStatus = 0;
                await _unitOfWork.Save();
                // call ProductQueueService to push product to approval queue
                await _productQueueService.AddProductToQueueAsync(product, "price more than 5000 at the time of update", "UPDATE");

                return 2;

            }else if((product.ProductPrice - productToBeUpdated.ProductPrice) / productToBeUpdated.ProductPrice > 0.5)
            {
                //5.	Any product should be pushed to approval queue if its price is more than 50% of its previous price.
                productToBeUpdated.ProductStatus = 0;
                await _unitOfWork.Save();
                await _productQueueService.AddProductToQueueAsync(product, "price more than 50% of its previous price", "UPDATE");
                return 3;
            }
            else
            {
                // if don't need to push to queue, then just update ProdouctDetail database
                productToBeUpdated.ProductName = product.ProductName;
                productToBeUpdated.ProductDescription = product.ProductDescription;
                productToBeUpdated.ProductPrice = product.ProductPrice;
                productToBeUpdated.ProductStatus = product.ProductStatus;
                productToBeUpdated.ProductPostDate = product.ProductPostDate;
                await _unitOfWork.Save();
                return 1;
            }
        }

        public async Task<int> DeleteProduct(int id)
        {

            //6.	Product should be pushed to approval queue in case delete.
            var productToBeDelete = await _unitOfWork.Products.GetByIdAsync(id);

            // firstly check product status, if status is 0, then will refuce this operation cause this product may be waiting for approval in queue
            if (productToBeDelete.ProductStatus == 0)
            {
                // "5" means this request cannot be handled now cause status is inactive
                return 5;
            }

            productToBeDelete.ProductStatus = 0;
            await _unitOfWork.Save();
            await _productQueueService.AddProductToQueueAsync(productToBeDelete, "operation for product deleting", "DELETE");
            return 4;
        }

        //User can search using Product name, Price range and posted date range
        public async Task<IEnumerable<ProductDetail>> SearchProducts(string? ProductName, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate)
        {
            if(MinPrice == null)
            {
                MinPrice = 0;
            }
            if(MaxPrice == null) {  
                MaxPrice = int.MaxValue; 
            }
            if(StartDate == null)
            {
                StartDate = DateTime.Parse("1900-01-01");
            }
            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }
            var ProductSet =  await _unitOfWork.Products.SearchProductsByQueryAsync(ProductName, MinPrice, MaxPrice, StartDate, EndDate);
            if(ProductSet != null && ProductSet.Count() >  0) {
                ProductSet.Where(p => p.ProductStatus == 1).OrderByDescending(p => p.ProductPostDate);
            }
            
            return ProductSet;
        }


    }
}
