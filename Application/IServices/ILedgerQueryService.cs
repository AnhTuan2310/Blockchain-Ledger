using Application.DTOs.Blocks;

namespace Application.IServices {
    public interface ILedgerQueryService {
        Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync();
        Task<bool> ValidateChainAsync();
        Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword);
        Task<List<BlockDTO>> GetCurrentChainAsync();
        Task<BlockDTO?> GetBlockDetailAsync(string hash);
    }
}
