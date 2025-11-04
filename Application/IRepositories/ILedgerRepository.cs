using Application.DTOs.Ledger;
using Domain.Entities;

namespace Application.IRepositories {
    public interface ILedgerRepository {
        Task AddBlockAsync(Block block);
        Task<SnapshotDTO> CreateSnapshotAsync(string description, List<Block> chain, List<Transaction> mempool);
        Task<SnapshotDTO?> GetSnapshotAsync(Guid snapshotId);
        Task RollbackToSnapshotAsync(SnapshotDTO snapshotDto);
        Task<List<Block>> GetCurrentChainAsync();
    }
}
