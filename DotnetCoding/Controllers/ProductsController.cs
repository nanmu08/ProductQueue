using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;
using DotnetCoding.Models;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var productDetailsList = await _productService.GetAllProducts();
            if(productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var productDetail = await _productService.GetProductById(id);
            if(productDetail == null)
            {
                return NotFound();
            }
            return Ok(productDetail);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductVM productvm)
        {
            //match productvm to productdetail
            ProductDetail product = new ProductDetail();
            product.ProductName = productvm.ProductName;
            product.ProductDescription = productvm.ProductDescription;
            product.ProductPrice = productvm.ProductPrice;


            // 3.Product creation is not allowed when its price is more than 10000 dollars
            if (product.ProductPrice > 10000)
            {
                return BadRequest("price larger than 10000 dollars not allowed");
            }
            var productHasAddedResult = await _productService.AddProduct(product);
            if(productHasAddedResult == 2)
            {
                return Accepted("Push to Approval Queue Cause price more than 5000 at the time of create");
            }
            else
            {
                return Ok("Add to ProductDetail Table successfully");
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductVM productvm, [FromRoute] int id)
        {
            //match productvm to productdetail
            ProductDetail product = new ProductDetail();
            product.Id = id;
            product.ProductName = productvm.ProductName;
            product.ProductDescription = productvm.ProductDescription;
            product.ProductPrice = productvm.ProductPrice;


            var UpdateResult = await _productService.UpdateProduct(product);
            //var productDetail = await _productService.GetProductById(product.Id);
            //if (productDetail == null)
            //{
            //    return BadRequest();
            //}
            //return Ok(productDetail);

            if(UpdateResult == 2)
            {
                return Accepted("Push to Approval Queue Cause price more than 5000 at the time of update");

            }else if(UpdateResult== 3)
            {
                return Accepted("Push to Approval Queue Cause price more than 50% of its previous price");

            }else if(UpdateResult == 5)
            {
                return BadRequest("Cannot handle now cause product status is 0");
            }
            else
            {
                return Ok("Update to ProductDetail Table successfully");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var productDetail = await _productService.GetProductById(id);
            if(productDetail == null)
            {
                return NotFound();
            }
            var deleteResult = await _productService.DeleteProduct(id);
            if (deleteResult == 4)
            {
                return Accepted("Push to Approval Queue Cause This is Delete Operation");

            }else if(deleteResult == 5)
            {
                return BadRequest("Cannot handle now cause product status is 0");
            }
            else
            {
                return NoContent();
            }
            
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string? ProductName, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate)
        {
            IEnumerable<ProductDetail> ProductSet = await _productService.SearchProducts(ProductName, MinPrice, MaxPrice, StartDate, EndDate);
            if (ProductSet == null || ProductSet.Count() == 0)
            {
                return NotFound();
            }
            
            return Ok(ProductSet);
        }
    }
}
