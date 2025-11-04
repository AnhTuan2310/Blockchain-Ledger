using Application.DTOs;
using Application.DTOs.Ledger;
using Domain.Entities;

namespace Application.IServices {
    public interface ILedgerQueryService {
        Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync();
        Task<bool> ValidateChainAsync();
        Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword);
        Task<List<Block>> GetCurrentChainAsync();
    }
}
