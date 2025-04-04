using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.Models;
using OnlineBanking.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace OnlineBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("processtransaction")]
        [RequestSizeLimit(100_000)] // Limit request size
        [RequestFormLimits(MultipartBodyLengthLimit = 64_000)] // Multipart request size
       
        public async Task<IActionResult> ProcessTransaction([FromBody] TransactionRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid transaction request.");
                }

                // Process the transaction
                var transactionResult = await Task.Run(() => _transactionService.ProcessTransaction(request));

                // Check the result of the transaction processing
                if (transactionResult.IsSuccess)
                {
                    return Ok(transactionResult);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, transactionResult.Message);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the transaction.");
            }
        }

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();
        //    services.AddScoped<ITransactionService, TransactionService>();
        //    // Other service registrations
        //}
    }
}
   
