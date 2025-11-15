using Api.Models;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {
    [ApiController]
    [Route("api/ledger/[controller]")]
    public class BlocksController : ControllerBase {
        private readonly ILedgerService _ledgerService;
        private readonly ILedgerQueryService _ledgerQueryService;

        public BlocksController(ILedgerService ledgerService, ILedgerQueryService ledgerQueryService) {
            _ledgerService = ledgerService;
            _ledgerQueryService = ledgerQueryService;
        }

        // GET /api/ledger/blocks
        [HttpGet]
        public async Task<IActionResult> Get() {
            var result = await _ledgerQueryService.GetBlockSummariesAsync();
            return Ok(ApiResponse<object>.Success(result, "Fetched blockchain ledger"));
        }

        // POST /api/ledger/blocks/mine
        [HttpPost("mine")]
        public async Task<IActionResult> MineBlock([FromBody] MineBlockRequest req) {
            if (req == null || string.IsNullOrWhiteSpace(req.MinerNote))
                return BadRequest(ApiResponse<string>.Failure("MinerNote is required."));

            var block = await _ledgerService.MineBlockAsync(req.MinerNote);
            return Ok(ApiResponse<object>.Success(block, "Block mined successfully"));
        }

        // GET /api/ledger/blocks/search?keyword=xxx
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(ApiResponse<string>.Failure("Keyword is required."));

            var result = await _ledgerQueryService.SearchBlocksAsync(keyword);
            return Ok(ApiResponse<object>.Success(result, $"Search results for '{keyword}'"));
        }
    }

    public class MineBlockRequest {
        public string MinerNote { get; set; } = string.Empty;
    }
}
