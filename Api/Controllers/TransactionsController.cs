using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers {
    [ApiController]
    [Route("api/ledger/[controller]")]
    public class TransactionsController : ControllerBase {
        private readonly ILedgerService _ledgerService;

        public TransactionsController(ILedgerService ledgerService) {
            _ledgerService = ledgerService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction tx) {
            if (tx == null)
                return BadRequest(ApiResponse<string>.Failure("Transaction payload is required"));
            // Trước kia là return BadRequest(...);

            await _ledgerService.AddTransactionAsync(tx);
            return Ok(ApiResponse<object>.Success(tx, "Transaction added to pending pool"));
            // Trước kia là return Ok(tx);
        }
    }
}
