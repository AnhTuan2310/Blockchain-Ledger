using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers {
    [ApiController]
    [Route("api/ledger/[controller]")]
    public class ChainController : ControllerBase {
        private readonly ILedgerService _ledgerService;
        private readonly ILedgerQueryService _ledgerQueryService;

        public ChainController(ILedgerService ledgerService, ILedgerQueryService ledgerQueryService) {
            _ledgerService = ledgerService;
            _ledgerQueryService = ledgerQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChain() {
            var chain = await _ledgerService.GetCurrentChainAsync();
            return Ok(ApiResponse<object>.Success(chain, "Current blockchain loaded"));
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate() {
            var ok = await _ledgerQueryService.ValidateChainAsync();
            return Ok(ApiResponse<bool>.Success(ok, ok ? "Chain valid" : "Chain broken"));
        }

        public class SnapshotRequest { public string Description { get; set; } = string.Empty; }

        [HttpPost("snapshot")]
        public async Task<IActionResult> CreateSnapshot([FromBody] SnapshotRequest req) {
            var dto = await _ledgerService.CreateSnapshotAsync(req.Description);
            return Ok(ApiResponse<object>.Success(dto, "Snapshot created"));
        }

        [HttpPost("rollback/{id:guid}")]
        public async Task<IActionResult> Rollback([FromRoute] Guid id) {
            await _ledgerService.RollbackToSnapshotAsync(id);
            return Ok(ApiResponse<object>.Success(id, "Rolled back to snapshot"));
        }
    }
}
