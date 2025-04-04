
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnlineBanking.Models;
using OnlineBanking.Services;

namespace OnlineBanking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("Token")] // Apply specific rate limit
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("balance/{accountId}")]
        public ActionResult<decimal> GetBalance(int accountId)
        {
            var balance = _accountService.GetBalance(accountId);
            if (balance.HasValue)
                return Ok(balance.Value);
            return NotFound("Account not found");
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            var result = _accountService.Transfer(request.FromAccount, request.ToAccount, request.Amount);
            if (result) return Ok("Transfer successful");
            return BadRequest("Transfer failed");
        }
    }
}
