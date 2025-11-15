
using Application.DTOs.Snapshot;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IRepositories {
    public interface ILedgerRepository {
        Task AddBlockAsync(Block block);

        // Thêm dòng này:
        Task<List<Block>> GetCurrentChainAsync();

        Task<SnapshotDTO> CreateSnapshotAsync(
            string description,
            List<Block> chain,
            List<Transaction> mempool);

        Task<MemoryRecord?> GetSnapshotAsync(Guid id);
        Task RollbackToSnapshotAsync(MemoryRecord record);
    }
}
