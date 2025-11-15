using Api.Models;
using Application.DTOs.Transactions;
using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {
    [ApiController]
    [Route("api/ledger/[controller]")]
    public class TransactionsController : ControllerBase {
        private readonly ILedgerService _ledgerService;

        public TransactionsController(ILedgerService ledgerService) {
            _ledgerService = ledgerService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] CreateTransactionRequestDTO dto) {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Failure(
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var tx = new Transaction {
                Content = dto.Content,
                Author = dto.Author,
                Timestamp = DateTime.UtcNow
            };

            await _ledgerService.AddTransactionAsync(tx);

            // Style B: không trả full data, chỉ message
            return Ok(ApiResponse<string>.Success(null, "Transaction added to pending pool"));
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTransactionRequestDTO dto) {
            if (id != dto.Id)
                return BadRequest(ApiResponse<string>.Failure("Route id must match body Id."));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Failure(
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var updated = await _ledgerService.UpdateTransactionAsync(dto);
            // Nếu có extension ToDTO(): trả về DTO cho client
            return Ok(ApiResponse<object>.Success(updated.ToDTO(), "Transaction updated successfully"));
        }

        // NEW: DELETE /api/ledger/transactions/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) {
            await _ledgerService.DeleteTransactionAsync(id);
            return Ok(ApiResponse<string>.Success(null, "Transaction deleted from pending pool"));
        }
    }
}
