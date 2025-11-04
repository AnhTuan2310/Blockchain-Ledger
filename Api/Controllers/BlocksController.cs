using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Api.Models;

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
        [HttpGet]
        public async Task<IActionResult> GetBlocks() {
            var blocks = await _ledgerService.GetCurrentChainAsync();
            return Ok(ApiResponse<object>.Success(blocks, "Fetched blockchain ledger"));
            //Trước kia là return Ok(blocks);
        }
        public class MineBlockRequest { public string MinerNote { get; set; } = string.Empty; }

        [HttpPost("mine")]
        public async Task<IActionResult> MineBlock([FromBody] MineBlockRequest req) {
            if (req == null || string.IsNullOrWhiteSpace(req.MinerNote))
                return BadRequest(ApiResponse<string>.Failure("MinerNote is required."));
                //Trước kia là return BadRequest(...);

            var block = await _ledgerService.MineBlockAsync(req.MinerNote);
            return Ok(ApiResponse<object>.Success(block, "Block mined successfully"));
            //Trước kia là return Ok(block);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(ApiResponse<string>.Failure("Keyword is required."));
                //Trước kia là return BadRequest(...);

            var result = await _ledgerQueryService.SearchBlocksAsync(keyword);
            return Ok(ApiResponse<object>.Success(result, $"Search results for '{keyword}'"));
            //Trước kia là return Ok(result);
        }
    }
}
