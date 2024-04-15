using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;
using DotnetCoding.Models;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalQueueController : ControllerBase
    {
        public readonly IProductQueueService _productQueueService;
        public ApprovalQueueController(IProductQueueService productQueueService)
        {
            _productQueueService = productQueueService;
        }

        /// <summary>
        /// Get the list of product in Approval Queue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var productQueueList = await _productQueueService.GetProductQueue();
            if (productQueueList == null)
            {
                return NotFound();
            }
            return Ok(productQueueList);
        }

        [HttpPost("Process")]
        public async Task<IActionResult> ProcessQueue([FromBody] QueueDecision queueDecison)
        {
            int requestResult = -1;
            if(queueDecison.OperationApproveOrNot == true)
            {
                requestResult = await _productQueueService.ApproveOrNotForOperationRequest(queueDecison.ProductQueueId, 1);
            }
            else if(queueDecison.OperationApproveOrNot == false)
            {   
                requestResult = await _productQueueService.ApproveOrNotForOperationRequest(queueDecison.ProductQueueId, 0);
            }
            else
            {
                return BadRequest("Must Be Accept Or Refuse");
            }
            if(requestResult == 1)
            {
                return Ok("Request Has Been Approved");
                
            }
            else if (requestResult == 0)
            {
                return BadRequest("QueueId not Available");
            }
            else
            {
                return BadRequest();
            }
            
        }

    }
}